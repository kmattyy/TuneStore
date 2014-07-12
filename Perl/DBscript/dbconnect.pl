
#!/usr/bin/perl

# PERL MODULES WE WILL BE USING
use lib 'D:\Adatbazis\Project\Perl\Adatbazis';
use DBI;
use Insert;
use File::Find::Rule;

$string = "dbi:ODBC:dsn=project;Database=projecttest;Trusted_Connection=Yes";
my $mssql_dbh = DBI->connect($string)
  or die
"\n\nThe mssql connection died with the following error: \n\n$DBI::errstr\n\n";
print "Database connection OK!\n";

my $insert=Insert->new(dbconnection => $mssql_dbh);


if ( -d $ARGV[0] ){
	# directory $ARGV[0] exists
	print "Processing directory: ". $ARGV[0]."\n";
	my @mp3s = File::Find::Rule->name( '*.mp3' )->in( $ARGV[0] );
	
	foreach (@mp3s) {
 	#print $_ . "\n";
 	
 	my $file = $_ ;
 
    $file =~ s/\//\\/;
 	
 	print "Processing file: " .  $file. "\n";
 	my $result= Insert->InsertTrack("".$file."");
 }
 print "DONE"; 

}

elsif ( -e $ARGV[0] ){

	# $ARGV[0] exists but is not a directory
	#print $mssql_dbh."\n";
	print "Processing file: " . $ARGV[0] . "\n";
	my $result= Insert->InsertTrack($ARGV[0]);
    #print $result;
    
    
}
else {
	die "Wrong parameter!!";
}


