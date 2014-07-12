#!/usr/bin/perl -w
use strict;

##  preliminaries
use lib 'D:\Adatbazis\Project\Perl\Adatbazis';
use strict ;
use warnings ; 
no warnings qw( uninitialized numeric void ) ;
use MP3::Info ;
use File::Find::Rule;
use DBI;
use Insert;



print $ARGV[0]."\n";
print $ARGV[1]."\n";
print $ARGV[2]."\n";
#$string = "dbi:ODBC:dsn=project;Database=projecttest;Trusted_Connection=Yes";
my $string = "dbi:ODBC:dsn=project;Database=".$ARGV[1].";Trusted_Connection=Yes";
my $mssql_dbh = DBI->connect($string)
  or die
"\n\nThe mssql connection died with the following error: \n\n$DBI::errstr\n\n";
my $insert=Insert->new(dbconnection => $mssql_dbh);

print "Database connection OK!\n";
my $otfile=$ARGV[2];
my $playlistid=$ARGV[0];


#"SELECT adress, filename from tracks t ,paths p where t.pathid=p.pathid 

## get the MP3 files that we want to add to our playlist

my @infiles = $insert-> getTracksForPlaylist($playlistid);

## prepare our new M3U playlist
open( OTFILE, ">$otfile" ) || die "could not overwrite yes
: $!";
print OTFILE "#EXTM3U\n";

foreach my $infile (@infiles) {

    print $infile."\n";
    ## get info from the files
    my $mtag = get_mp3tag( $infile) ;
    my $info = get_mp3info($infile) ;

    my $artist = $mtag->{ARTIST} ; 
    my $stitle = $mtag->{TITLE} ; 

    ## create title-author pair
    my $pair ;

    if ( $stitle ne "" ) {
    $pair .= $stitle ; 
    }

    if ( $artist ne "" && $stitle ne "" ) {
    $pair .= " (by " . $artist . ")" ; 
    } elsif ( $artist ne "" ) {
    $pair .= $artist ;  
    }

    if ( $pair !~ /[A-Za-z0-9]/ ) {
    $pair = $infile ;  
    }

    ## prepare output line
    my $otline = "#EXTINF:" . int ($info->{SECS}) . "," . $pair ; 
    
    ## send output to our new M3U playlist
    print OTFILE $otline . "\n" ; 
    print OTFILE $infile . "\n" ;
    
}

##  close the M3U playlist
close OTFILE;

