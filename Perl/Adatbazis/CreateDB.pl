use lib 'D:\Adatbazis\Project\Perl\Adatbazis';
use DBI;
use Insert;
use File::Find::Rule;


my $dbname=shift;
print $dbname."\n";
#$string = "dbi:ODBC:dsn=project;Database=projecttest;Trusted_Connection=Yes";
$string = "dbi:ODBC:dsn=project;Trusted_Connection=Yes";
my $mssql_dbh = DBI->connect($string)
  or die
"\n\nThe mssql connection died with the following error: \n\n$DBI::errstr\n\n";
print "Server connected!\n";


my $myquery = "Drop DATABASE ".$dbname ;
my $sth = $mssql_dbh->prepare($myquery);

$sth->execute;
print "Database dropped!\n";

$myquery = "CREATE DATABASE ".$dbname ;
$sth = $mssql_dbh->prepare($myquery);

$sth->execute;

$string = "dbi:ODBC:dsn=project;database=$dbname; Trusted_Connection=Yes";
$mssql_dbh = DBI->connect($string)
  or die
"\n\nThe mssql connection died with the following error: \n\n$DBI::errstr\n\n";
print "Database created & connected OK!\n";


$myquery = " 
DROP TABLE playlisttracks
DROP TABLE playlists 
DROP TABLE tracks 
DROP TABLE albums 
DROP TABLE artists
DROP TABLE covers 
DROP TABLE genres
DROP TABLE keys 
DROP TABLE paths
DROP TABLE applog
DROP TABLE logtype
DROP TABLE quality"
;

$sth = $mssql_dbh->prepare($myquery);
$sth->execute ;#or die "Ok if db dropped successfully";


$myquery =
'
--1.[Path|PathID;Adress] 
CREATE TABLE paths 
  ( 
     pathid INT PRIMARY KEY IDENTITY, 
     adress VARCHAR(200) 
  ) 

--2.Covers 
CREATE TABLE covers 
  ( 
     coverid INT PRIMARY KEY IDENTITY, 
     name    VARCHAR(40), 
     pathid  INT REFERENCES paths(pathid), 
	 bindata VARBINARY(MAX) 
  ) 



--4.[Genres|GenreID;Name] 
CREATE TABLE genres 
  ( 
     genreid   INT PRIMARY KEY IDENTITY, 
     genrename VARCHAR(100) 
  ) 

  --3.[Artists|ArtistID;Name;BirthDate] 
CREATE TABLE artists 
  ( 
     artistid  INT PRIMARY KEY IDENTITY, 
     name      VARCHAR(200), 
     favgenre INT FOREIGN KEY REFERENCES genres(genreid) 
  ) 


--5.[Albums|AlbumID;Title;RelDate;ArtistID;PublisherID] 
CREATE TABLE Albums 
  ( 
     albumid  INT PRIMARY KEY IDENTITY, 
     Title    VARCHAR(200), 
     year     INT, 
     artistid INT FOREIGN KEY REFERENCES artists(artistid), 
    -- coverid  INT FOREIGN KEY REFERENCES covers(coverid) 
  ) 

--6.tags eg:ID3v2,ID3v1 
--CREATE TABLE tags 
--  ( 
--     tagid INT PRIMARY KEY IDENTITY, 
--     tag   VARCHAR(10) 
--  ) 


CREATE TABLE quality
  ( 
    qualityid INT PRIMARY KEY IDENTITY, 
    quality   VARCHAR(15) 
  )
  

CREATE TABLE keys
  ( 
    keyid INT PRIMARY KEY IDENTITY, 
    keymark   VARCHAR(10) 
  ) 

--7.[TrackID] 
CREATE TABLE tracks 
  ( 
     trackid   INT PRIMARY KEY IDENTITY, 
     title     VARCHAR(150),
     duration  VARCHAR(20),
    --duration  REAL, 
     --year      INT,
     year VARCHAR(30),
     bpm       INT, 
     comment   VARCHAR(500), 
     filename  VARCHAR(150),
     artistid  INT FOREIGN KEY REFERENCES artists(artistid),
     albumid   INT FOREIGN KEY REFERENCES albums(albumid),
     artworkid INT FOREIGN KEY REFERENCES covers(coverid), 
     genreid   INT FOREIGN KEY REFERENCES genres(genreid),
     keyid     INT FOREIGN KEY REFERENCES keys(keyid),
     qualityid INT FOREIGN KEY REFERENCES quality(qualityid), 
     pathid    INT FOREIGN KEY REFERENCES paths(pathid)
  ) 

--8.[Playlists] 
CREATE TABLE playlists 
  ( 
     playlistid   INT PRIMARY KEY IDENTITY, 
     playlistname VARCHAR(30), 
     pathid	  INT FOREIGN KEY REFERENCES paths(pathid), 
     createdate   DATETIME 
	-- timecount
  ) 

--9.[PlaylistTracks]
CREATE TABLE playlisttracks 
  ( 
     playlistid INT FOREIGN KEY REFERENCES playlists(playlistid), 
     trackid    INT FOREIGN KEY REFERENCES tracks(trackid) 
  ) 

CREATE TABLE logtype 
 ( 
      logtypeId  INT PRIMARY KEY IDENTITY, 
	 logtype VARCHAR(30) 
 ) 


CREATE TABLE applog 
  (
         logid INT PRIMARY KEY IDENTITY, 
	 info VARCHAR(MAX), 
	 createdate datetime ,
	 typeid INT FOREIGN KEY REFERENCES logtype(logtypeid)
  ) 

  Insert into paths(adress)values(\'D:\Adatbazis\Project\Covers\ \')

  Insert into genres(genrename) values(\'Not Set\')
  
  Insert into keys(keymark) values(\'Not Set\')

  Insert into artists(name, favgenre) values(\'Unknown Artist\',1)

  Insert into albums(title,year,artistid) values(\'Unknown Album\',0,1)

  Insert into logtype(logtype) values(\'Script\')

  Insert into logtype(logtype) values(\'Insert\')

  Insert into logtype(logtype) values(\'Update\')

  Insert into logtype(logtype) values(\'Delete\')
  
  Insert into quality(quality) values(\'High\')
  
  Insert into quality(quality) values(\'Medium\')
  
  Insert into quality(quality) values(\'Low\')

  Insert into playlists(playlistname,pathid,createdate) values(\'Default\',1,GETDATE())
  
  Insert into applog(info,createdate,typeid) values(\'Database created\',GETDATE(),1)
  ';
  $sth = $mssql_dbh->prepare($myquery);

    $sth->execute ;#or die "Bajvan";
    
    
    
$myquery='
CREATE VIEW tracksview 
AS 
  SELECT trackid, 
         a.name, 
         t.title, 
         g.genrename Genre, 
         al.title    Albumtitle, 
         k.keymark, 
         q.quality 
  FROM   tracks t, 
         artists a, 
         genres g, 
         albums al, 
         keys k, 
         quality q 
  WHERE  t.artistid = a.artistid 
         AND g.genreid = t.genreid 
         AND al.albumid = t.albumid 
         AND k.keyid = t.keyid 
         AND q.qualityid = t.qualityid 
 
  ';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";


#views triggers and stored procedures
$myquery='

--view for all track informaion
CREATE VIEW infoview 
AS 
  SELECT trackid, 
         t.title, 
         a.name, 
         al.title    Albumtitle, 
         g.genrename Genre, 
         k.keymark, 
         t.year, 
         t.bpm, 
         q.quality, 
         p.adress, 
         t.filename, 
         t.comment 
  FROM   tracks t, 
         artists a, 
         genres g, 
         albums al, 
         keys k, 
         quality q, 
         [paths] p 
  WHERE  t.artistid = a.artistid 
         AND g.genreid = t.genreid 
         AND al.albumid = t.albumid 
         AND k.keyid = t.keyid 
         AND q.qualityid = t.qualityid 
         AND t.pathid = p.pathid 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";



$myquery='
--view for playlistgrid
CREATE VIEW playlistview 
AS 
  SELECT t.trackid, 
         pl.playlistid, 
         a.name  Artist, 
         t.title Title 
  FROM   tracks t, 
         playlisttracks pl, 
         artists a 
  WHERE  t.trackid = pl.trackid 
         AND t.artistid = a.artistid 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='
--procedure for genre insert
CREATE PROCEDURE Insertgenre (@genre VARCHAR(50), 
                              @id    INT out) 
AS 
  BEGIN 
      SET nocount ON 

      INSERT INTO genres 
                  (genrename) 
      VALUES      (@genre) 

      SET @id=(SELECT @@IDENTITY AS \'Identity\') 
  END 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='
 --procedure for key insert
CREATE PROCEDURE Insertkey (@key VARCHAR(50), 
                            @id  INT out) 
AS 
  BEGIN 
      SET nocount ON 

      INSERT INTO keys 
                  (keymark) 
      VALUES      (@key) 

      SET @id=(SELECT @@IDENTITY AS \'Identity\') 
  END 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='

  CREATE PROCEDURE Updateartist (@trackid INT, 
                               @artist  VARCHAR(50), 
                               @id      INT out) 
AS 
  BEGIN 
      SET nocount ON 

      DECLARE @artistid INT 

      SET @artistid=(SELECT artistid 
                     FROM   tracks 
                     WHERE  trackid = @trackid) 

      UPDATE artists 
      SET    name = @artist 
      WHERE  artistid = @artistid 

      SET @id=@artistid 
  END 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='
CREATE PROCEDURE Updatealbum (@trackid INT, 
                              @album   VARCHAR(50), 
                              @id      INT out) 
AS 
  BEGIN 
      SET nocount ON 

      DECLARE @albumid INT 

      SET @albumid=(SELECT albumid 
                    FROM   tracks 
                    WHERE  trackid = @trackid) 

      UPDATE albums 
      SET    title = @album 
      WHERE  albumid = @albumid 

      SET @id=@albumid 
  END 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='
 CREATE PROCEDURE Insertartist (@artist VARCHAR(50), 
                                @id     INT out) 
AS 
  BEGIN 
      SET nocount ON 

      IF NOT EXISTS (SELECT 1 
                     FROM   artists 
                     WHERE  name = @artist) 
        BEGIN 
            INSERT INTO artists 
                        (name) 
            VALUES      (@artist) 

            SET @id=(SELECT @@IDENTITY AS \'Identity\') 
        END 
      ELSE 
        BEGIN 
            SET @id=(SELECT artistid 
                     FROM   artists 
                     WHERE  name = @artist) 
        END 
  END 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='

  CREATE PROCEDURE Insertalbum (@album VARCHAR(50), 
                             @id    INT out) 
AS 
  BEGIN 
      SET nocount ON 

      IF NOT EXISTS (SELECT 1 
                     FROM   albums 
                     WHERE  title = @album) 
        BEGIN 
            INSERT INTO albums 
                        (title, 
                         [year]) 
            VALUES      (@album, 
                         0) 

            SET @id=(SELECT @@IDENTITY AS \'Identity\') 
        END 
      ELSE 
        BEGIN 
            SET @id=(SELECT albumid 
                     FROM   albums 
                     WHERE  title = @album) 
        END 
  END 

';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='
--trigger for trackdelete
  CREATE TRIGGER trackdelete
ON tracks
instead OF DELETE
AS
  BEGIN
      SET nocount ON

      DELETE FROM playlisttracks
      WHERE  trackid IN (SELECT deleted.trackid
                         FROM   deleted)

      DELETE FROM tracks
      WHERE  trackid IN (SELECT deleted.trackid
                         FROM   deleted)

      DECLARE @inform     VARCHAR(400),
              @trackid    INT,
              @tracktitle VARCHAR(200)
      DECLARE kurzor CURSOR local FOR
        SELECT trackid,
               title
        FROM   deleted

      OPEN kurzor

      FETCH next FROM kurzor INTO @trackid, @tracktitle

      WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @inform=( \'Track: \' + @tracktitle + \' with id:\'
                          + Cast(@trackid AS VARCHAR(10)) + \' deleted\' )

            INSERT INTO applog
                        (info,
                         createdate,
                         typeid)
            VALUES      (@inform,
                         Getdate(),
                         4)

            FETCH next FROM kurzor INTO @trackid, @tracktitle
        END
  END 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='
  --trigger for albumdelete
  CREATE TRIGGER albumdelete 
ON albums 
instead OF DELETE 
AS 
  BEGIN 
      SET nocount ON 

      
      DELETE FROM tracks 
      WHERE  albumid IN (SELECT deleted.albumid 
                         FROM   deleted) 

      DELETE FROM albums 
      WHERE  albumid IN (SELECT deleted.albumid 
                         FROM   deleted) 

      DECLARE @inform     VARCHAR(400), 
              @albumid    INT, 
              @albumtitle VARCHAR(200) 
      DECLARE kurzor CURSOR local FOR 
        SELECT albumid, 
               title 
        FROM   deleted 

      OPEN kurzor 

      FETCH next FROM kurzor INTO @albumid, @albumtitle 

      WHILE @@FETCH_STATUS = 0 
        BEGIN 
            SET @inform=( \'Album:\' + @albumtitle + \' with id:\' 
                          + Cast(@albumid AS VARCHAR(10)) + \' deleted\' ) 
           
            INSERT INTO applog 
                        (info, 
                         createdate, 
                         typeid) 
            VALUES      (@inform, 
                         Getdate(), 
                         4) 

            FETCH next FROM kurzor INTO @albumid, @albumtitle 
        END 
  END 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='

  --trigger for artistdelete
  CREATE TRIGGER artistdelete 
ON artists 
instead OF DELETE 
AS 
  BEGIN 
      SET nocount ON 

      DELETE FROM tracks 
      WHERE  artistid IN (SELECT deleted.artistid 
                          FROM   deleted) 

      DELETE FROM albums 
      WHERE  artistid IN (SELECT deleted.artistid 
                          FROM   deleted) 

      DELETE FROM artists 
      WHERE  artistid IN (SELECT deleted.artistid 
                          FROM   deleted) 

      DECLARE @inform     VARCHAR(400), 
              @artistidid INT, 
              @artistname VARCHAR(200) 

      SET @artistidid=(SELECT deleted.artistid 
                       FROM   deleted) 
      SET @artistname=(SELECT deleted.name 
                       FROM   deleted) 
    
      SET @inform=( \'Artist: \' + @artistname + \' with id:\' 
                    + Cast(@artistidid AS VARCHAR(10)) 
                    + \' deleted\' ) 

      INSERT INTO applog 
                  (info, 
                   createdate, 
                   typeid) 
      VALUES      (@inform, 
                   Getdate(), 
                   4) 
  END 
';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='

  ------------artist favourite genre
  CREATE PROCEDURE Setfavgenre 
AS 
  BEGIN 
      SET nocount ON 

      DECLARE kurzor CURSOR local FOR 
        SELECT artistid 
        FROM   artists 
      DECLARE @id      INT, 
              @genreid INT 

      OPEN kurzor 

      FETCH next FROM kurzor INTO @id 

      WHILE @@FETCH_STATUS = 0 
        BEGIN 
            SET @genreid=(SELECT TOP 1 genreid 
                          FROM   tracks 
                          WHERE  artistid = @id 
                          GROUP  BY genreid) 

            UPDATE artists 
            SET    favgenre = @genreid 
            WHERE  artistid = @id 

            FETCH next FROM kurzor INTO @id 
        END 

      CLOSE kurzor 
  END
  ';
$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Bajvan";




$myquery='
  --set null genreid in tracks

  CREATE PROCEDURE Setnulls 
AS 
  BEGIN 
      --set nocount on 
      SELECT trackid 
      INTO   #temp 
      FROM   tracks 
      WHERE  genreid IS NULL 

      UPDATE tracks 
      SET    genreid = 1 
      WHERE  trackid IN (SELECT * 
                         FROM   #temp) 
  END 

';


$sth = $mssql_dbh->prepare($myquery);

$sth->execute ;#or die "Proc2";



#open IMAGE,'Album.jpg' or die $!;
open IMAGE,'D:\Adatbazis\Project\Perl\Adatbazis\no_artwork.jpg' or die $!;
binmode IMAGE;
my ($image, $buff);
while(read IMAGE, $buff, 1024) {
    $image .= $buff;
}
close(IMAGE);

$myquery ='Insert into covers(name,pathid,bindata)values(\'nopic\',1,?)';

#print length($image);
$sth = $mssql_dbh->prepare($myquery);
#$sth->bind_param(1 , $image , DBI::SQL_VARBINARY);
$sth->execute($image) or die "bajvan";

close(IMAGE);
$sth->finish();

print "Database done\n";


