package Insert;

use DBI;
use strict;
use warnings;
use MP3::Tag;
use MP3::Info;
use Math::BigFloat;

#our $string="dbi:ODBC:dsn=project;Database=projecttest;Trusted_Connection=Yes";

our $mssql_dbh;


#= DBI -> connect($string) or die "\n\nthe mssql connection died with the following error: \n\n$DBI::errstr\n\n";
#print "Success!!";

sub new {

	my ( $class, %args ) = @_;

	my $self = bless( {}, $class );

	my $db = exists $args{dbconnection} or die "\n No dbconnection!";
	$mssql_dbh = $args{dbconnection};
     
	#print $mssql_dbh."\n";
	#print"Insert obj OK!\n";
	return $self;
}

#album artist cover genre path tag tracks
sub InsertTrack {
	my $self = shift;
	my $file = shift;
	my $userid =shift;
	print $userid."\n";
	my $mp3  = MP3::Tag->new($file);
	#print $file. "\n";
	$mp3->get_tags;

	my $filename_nodir = $mp3->filename_nodir();
	my $abs_dirname    = $mp3->dirname();
	my $temppath       = $self->getPathId("$abs_dirname\\");
	my $myquery = "Select trackid from tracks where pathid=? AND filename=? ";
	my $sth     = $mssql_dbh->prepare($myquery);

	$sth->execute( $temppath, $filename_nodir )
	  or die "SQL Error: $DBI::errstr\n";

	if ( $sth->rows ) {
		print $filename_nodir. " Already exits in database. SKIPPED!\n";
		return 0;
	}

	if ( exists $mp3->{ID3v2} ) {

		# read some information from the tag if exist id3v2 tag
		my $name;
		my $info;
		my $artistid;
		( $name, $info ) = $mp3->{ID3v2}->get_frame("TPE1");    #artist
		if ( !defined($name) ) {
			$artistid = 1;                                      #unknown
		}
		else {
			$artistid = $self->InsertArtist($name);
		}

		my $year;
		my $trackyear;
		( $year, $info ) = $mp3->{ID3v2}->get_frame("TDRC");    #year

		if ( !defined($year) ) {
			$trackyear = 0;
		}
		else {
			$trackyear = $year;
		}

		my $album;
		my $albumid;

		( $album, $info ) = $mp3->{ID3v2}->get_frame("TALB");    #album
		if ( defined($album) ) {
			$albumid = $self->InsertAlbum( $album, $trackyear, $artistid );
		}
		else {
			$albumid = 1;
		}

		my $cover;
		( $cover, $info ) = $mp3->{ID3v2}->get_frame("APIC");    #cover
		my $artworkid;

		$artworkid = $self->CreateCover($cover);

		#$artworkid = $self->GetCoverIDbyalbumid($albumid);

		#$artworkid = 1;

		my $genre;
		my $genreid;
		( $genre, $info ) = $mp3->{ID3v2}->get_frame("TCON");    #genre

		if ( defined($genre) ) {
			$genreid = $self->InsertGenre($genre);
		}

		my $title;
		( $title, $info ) = $mp3->{ID3v2}->get_frame("TIT2");    #title
		if ( !defined($title) ) {
			$title = 'Missing title';
		}

		my $bpm;
		( $bpm, $info ) = $mp3->{ID3v2}->get_frame("TBPM");      #BPM
		if ( !defined($bpm) ) {
			$bpm = 0;
		}

		my $key;
		( $key, $info ) = $mp3->{ID3v2}->get_frame("TKEY");    #KEY
		                                                       #print $key."\n";
		my $keyid = $self->getKeyId($key);

		my $comment = $mp3->comment();

		my $tagid = 1;
		my $infos = get_mp3info($file);

		my $duration = $infos->{SECS};
		my $bitrate  = $infos->{BITRATE};
		my $mode     = $infos->{MODE};

		my $qualityid = $self->defineQuality( $bitrate, $mode );

		#print $filename_nodir. "\n";

		#print $abs_dirname. "\n";
		my $pathid = $self->InsertPath("$abs_dirname\\");

#[title]duration][year][bpm][comment][filename][artistid][albumid][artworkid][genreid][tagid][pathid]

		my $myquery =
"Insert into tracks(title,duration,year,bpm,comment,filename,artistid,albumid,artworkid,genreid,pathid,keyid,qualityid,userID)"
		  . " values(?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
		my $sth = $mssql_dbh->prepare($myquery);

		$sth->execute(
			$title,     $duration,       $trackyear, $bpm,
			$comment,   $filename_nodir, $artistid,  $albumid,
			$artworkid, $genreid,        $pathid,    $keyid,
			$qualityid, $userid
		  )

		  or print "ERROR: " . $title . " "
		  . $duration . " "
		  . $trackyear . " "
		  . $bpm . " "
		  . $comment . " "
		  . $filename_nodir . " "
		  . $artistid . " "
		  . $albumid . " "
		  . $artworkid . " "
		  . $genreid . " "
		  . $pathid . " "
		  . $qualityid . " "
		  . $keyid. " \n";

	}
	else {

		# create a new tag
		$mp3->new_tag("ID3v2");
		$mp3->{ID3v2}->add_frame( "TALB", "Album title" );
		$mp3->{ID3v2}->write_tag;
	}
	$sth->finish;
	return 0;
}

sub defineQuality {
	my $self    = shift;
	my $bitrate = shift;
	my $mode    = shift;

	#print $bitrate." ".$mode."\n";

	if ( $mode == 3 ) {    #single channel
		return 3;
	}
	else {
		if ( $bitrate < 200 ) {    #low bitrate medium
			return 2;
		}
		return 1;                  #otherwise high
	}
	#return 1;

}

sub InsertArtist {
	my $self   = shift;
	my $artist = shift;

	my $id = $self->getArtistId($artist);

	if ( $id == 0 ) {

		my $myquery = "Insert into artists(name) values(?)";
		my $sth     = $mssql_dbh->prepare($myquery);

		$sth->execute($artist)
		  or die "SQL Error: $DBI::errstr\n";
	}
	$id = $self->getArtistId($artist);
	return $id;
}

sub getArtistId {

	my $self   = shift;
	my $artist = shift;
	my $id;

	my $myquery = "select artistid from artists where name=?";

	my $sth = $mssql_dbh->prepare($myquery);

	$sth->execute($artist)
	  or die "SQL Error: $DBI::errstr\n";
	if ( $sth->rows ) {
		$sth->bind_columns( undef, \$id );
		$sth->fetch();
	}
	else {
		$id = 0;
	}

	return $id;
}

sub GetCoverIDbyalbumid {

	my $self    = shift;
	my $albumid = shift;
	my $id;
	my $myquery = "select coverid from albums where albumid=?";

	my $sth = $mssql_dbh->prepare($myquery);

	$sth->execute($albumid)
	  or die "SQL Error: $DBI::errstr\n";
	if ( $sth->rows ) {
		$sth->bind_columns( undef, \$id );
		$sth->fetch();
	}
	else {
		$id = 1;
	}
	return $id;
}

sub InsertAlbum {
	my $self     = shift;
	my $title    = shift;
	my $year     = shift;
	my $artistid = shift;
	my $id       = $self->getAlbumId( $title, $artistid );

	if ( $id == 0 ) {
		my $myquery = "Insert into albums(title,year,artistid) values(?,?,?)";
		my $sth     = $mssql_dbh->prepare($myquery);

		#print $title." \n".$year." \n". $artistid." \n". $coverid." \n";

		$sth->execute( $title, $year, $artistid );

		#or die "SQL Error: $DBI::errstr\n";
	}
	$id = $self->getAlbumId( $title, $artistid );
	return $id;
}

sub getAlbumId {

	my $self   = shift;
	my $title  = shift;
	my $artist = shift;
	my $id;

	my $myquery = "select albumid from albums where title=? ";

	my $sth = $mssql_dbh->prepare($myquery);

	#print $title. "\n";
	$sth->execute($title)
	  or die "SQL Error: $DBI::errstr\n";
	if ( $sth->rows ) {
		$sth->bind_columns( undef, \$id );
		$sth->fetch();
	}
	else {
		$id = 0;
	}

	return $id;
}

sub CreateCover {
	my $self  = shift;
	my $cover = shift;
	my $maxid;
	my $id;

	if ( !$cover->{_Data} ) {
		return 1;
	}

	my $myquery = "select coverid from covers where bindata = ?";

	my $sth = $mssql_dbh->prepare($myquery);

	$sth->execute( $cover->{_Data} )
	  or die "SQL Error: $DBI::errstr\n";

	if ( $sth->rows ) {
		$sth->bind_columns( undef, \$id );
		$sth->fetch();
		return $id;
	}

	$myquery = "select max(coverid) from covers ";

	$sth = $mssql_dbh->prepare($myquery);

	$sth->execute()
	  or die "SQL Error: $DBI::errstr\n";

	$sth->bind_columns( undef, \$maxid );
	$sth->fetch();

	#if ($cover->{_Data}=)

	#	for (keys %cover)
	#	 {
	#	if ($_!= "_Data") {
	#        print $hash{$_};
	#	}
	#
	#	  }

	my $filename = ( $maxid + 1 ) . ".jpg";
	binmode STDOUT;

	#print $cover->{"Picture Type"};
	open( SAVE, ">D:\\Adatbazis\\Project\\Covers\\" . $filename );
	binmode SAVE;
	print SAVE $cover->{_Data};
	close SAVE;

	$myquery = "insert into covers(name,pathid,bindata)values(?,1,?) ";

	$sth = $mssql_dbh->prepare($myquery);

	$sth->execute( $filename, $cover->{_Data} )
	  or die "SQL Error: $DBI::errstr\n";

	return ( $maxid + 1 );
}

sub InsertGenre {
	my $self  = shift;
	my $genre = shift;

	my $id = $self->getGenreId($genre);

	if ( $id == 0 ) {

		my $myquery = "Insert into genres(genrename) values(?)";
		my $sth     = $mssql_dbh->prepare($myquery);

		$sth->execute($genre)
		  or die "SQL Error: $DBI::errstr\n";
	}
	$id = $self->getGenreId($genre);
	return $id;
}

sub getGenreId {

	my $self  = shift;
	my $genre = shift;
	my $id;

	my $myquery = "select genreid from genres where genrename=?";

	my $sth = $mssql_dbh->prepare($myquery);

	#print "\n".$genre."\n";

	$sth->execute($genre)    ##!!
	  or die "SQL Error: $DBI::errstr\n";
	if ( $sth->rows ) {
		$sth->bind_columns( undef, \$id );
		$sth->fetch();
	}
	else {
		$id = 0;
	}

	return $id;
}

sub InsertPath {
	my $self = shift;
	my $path = shift;

	my $id = $self->getPathId($path);

	if ( $id == 0 ) {

		my $myquery = "Insert into paths(adress) values(?)";
		my $sth     = $mssql_dbh->prepare($myquery);

		$sth->execute($path)
		  or die "SQL Error: $DBI::errstr\n";
	}
	$id = $self->getPathId($path);
	return $id;
}

sub getPathId {

	my $self = shift;
	my $path = shift;
	my $id;

	my $myquery = "select pathid from paths where adress=?";

	my $sth = $mssql_dbh->prepare($myquery);

	$sth->execute($path)
	  or die "SQL Error: $DBI::errstr\n";
	if ( $sth->rows ) {
		$sth->bind_columns( undef, \$id );
		$sth->fetch();
	}
	else {
		$id = 0;
	}

	return $id;
}

sub getKeyId {
	my $self = shift;
	my $key  = shift;

	my $id;
	if ( defined($key) ) {

		my $myquery = "select keyid from keys where keymark=?";

		my $sth = $mssql_dbh->prepare($myquery);

		$sth->execute($key)
		  or die "SQL Error: $DBI::errstr\n";
		if ( $sth->rows ) {
			$sth->bind_columns( undef, \$id );
			$sth->fetch();
			return $id;
		}
		else {

			$myquery = "insert into keys(keymark) values (?)";

			$sth = $mssql_dbh->prepare($myquery);

			$sth->execute($key);

			$myquery = "select keyid from keys where keymark=?";

			$sth = $mssql_dbh->prepare($myquery);

			$sth->execute($key);

			$sth->bind_columns( undef, \$id );

			$sth->fetch();

			return $id;

		}
	}
	else {
		return 1;
	}
}

sub getTracksForPlaylist {

	my $self   = shift;
	my $playlistid = shift;
	my $path;
	my $filename;
	my @tracks;

	my $myquery = "SELECT  distinct(filename),adress from tracks t ,paths p,playlisttracks pl where t.pathid=p.pathid and t.trackid=pl.trackid and pl.playlistid=? ";

	my $sth = $mssql_dbh->prepare($myquery);

	$sth->execute($playlistid)
	  or die "SQL Error: $DBI::errstr\n";
	  

	$sth->bind_columns(undef, \$filename, \$path);


	while($sth->fetch()) {
	 #print "$path, $filename \n";
	push(@tracks,$path."".$filename)
	} 
return @tracks;

}

return 1;
