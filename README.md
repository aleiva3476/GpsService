# GpsService
Service to collect readings of GPS devices and record them in a PostgreSQL database

#### Configuration parameters that must be set before compiling
* GpsService class
    * SERVICE_PORT: port that listens to requests
* Auxiliar/AuxSql.cs:
    * DB_HOST: host's URL
    * DB_DATABASE: database name
    * DB_USER: database's user
    * DB_PASSWORD: user's password
    * DB_PORT: server's port
* Direcciones/ConsultaGMaps.cs:
    * GOOGLE_API_KEY: google api key for maps

#### Database tables
You will find the structure of the tables in x_DB_SQL_x/GEO.sql

#### Installer
A file to create an installer with NSIS: x_NSIS_Installer_x/GPSService.nsi

#### How does it work
* The service receives a message
* Check if it is a valid message (Teltonika, Meiliago, Syrus)
* If it's not valid, inserts a record in the 'gps_error' table
* If it's correct, inserts a record in the 'gps' table
* Looks in the 'direcciones' table for a record within a 5 meter radius of the GPS location
* If it does not exist, reads from Google Maps the address corresponding to the location and inserts it into the 'direcciones' table
* Associates the 'gps' record with the 'direcciones' record

#### PostgreSQL requirements
* Support for jsonb ( version >= 9.4 )
* PostGIS module (http://postgis.net)

#### Acknowledgment
Meiligao proccess is based on: https://github.com/brimzi/meitrack-protocols
