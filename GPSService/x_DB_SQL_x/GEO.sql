CREATE EXTENSION IF NOT EXISTS postgis WITH SCHEMA public;
COMMENT ON EXTENSION postgis IS 'PostGIS geometry, geography, and raster spatial types and functions';


CREATE TABLE direcciones (
    id serial PRIMARY KEY,
    posicion geometry(Point,4326),
    calle character varying(100) DEFAULT ''::character varying NOT NULL,
    numero character varying(10) DEFAULT ''::character varying NOT NULL,
    localidad character varying(100) DEFAULT ''::character varying NOT NULL,
    provincia character varying(50) DEFAULT ''::character varying NOT NULL,
    cpostal character varying(12) DEFAULT ''::character varying NOT NULL,
    pais character varying(50) DEFAULT ''::character varying NOT NULL
);
ALTER TABLE direcciones OWNER TO postgres;


CREATE TABLE gps (
    id serial PRIMARY KEY,
    imei character varying(32) DEFAULT ''::character varying NOT NULL,
    fecha timestamp without time zone,
    posicion geometry(Point,4326),
    velocidad numeric(14,6) DEFAULT 0 NOT NULL,
    datos jsonb,
    hora timestamp without time zone DEFAULT now() NOT NULL,
    direccion integer DEFAULT 0 NOT NULL
);
ALTER TABLE gps OWNER TO postgres;
COMMENT ON COLUMN gps.id IS 'Identificador del registro';
COMMENT ON COLUMN gps.imei IS 'IMEI del localizador';
COMMENT ON COLUMN gps.fecha IS 'Fecha / hora de recepción';
COMMENT ON COLUMN gps.posicion IS 'Posición del localizador (long, lat)';
COMMENT ON COLUMN gps.velocidad IS 'Velocidad';
COMMENT ON COLUMN gps.datos IS 'Datos del localizador ("alt":altitud, "dat":sensores, "dir":dirección, "sat":satélite)';
COMMENT ON COLUMN gps.hora IS 'Hora de recepción de los datos';
COMMENT ON COLUMN gps.direccion IS 'Identificador del registro de dirección';


CREATE TABLE gps_error (
    id serial PRIMARY KEY,
    datos bytea,
    hora timestamp without time zone DEFAULT now() NOT NULL
);
ALTER TABLE gps_error OWNER TO postgres;
COMMENT ON COLUMN gps_error.id IS 'Identificador del registro';
COMMENT ON COLUMN gps_error.datos IS 'Datos recibidos';
COMMENT ON COLUMN gps_error.hora IS 'Hora de recepción';


CREATE INDEX idx_direcc_posicion_gist ON direcciones USING gist (posicion);
CREATE INDEX idx_gps_location_gist ON gps USING gist (posicion);
CREATE INDEX idx_gpx_imei ON gps USING btree (imei);
CREATE INDEX idx_gpx_imei_fecha ON gps USING btree (imei, fecha);
