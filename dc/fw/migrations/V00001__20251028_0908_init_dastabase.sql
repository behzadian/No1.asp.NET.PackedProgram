CREATE SEQUENCE id
    START WITH 1000
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


CREATE TABLE bank_account (
  id BIGINT PRIMARY KEY DEFAULT nextval('id'),
  balance_last_update_seconds int8,
  balance_last_update_nanoseconds int4,
  balance_last_update_timestamp timestamp,
  created_seconds int8,
  created_nanoseconds int4,
  created_timestamp timestamp,
  updated_seconds int8,
  updated_nanoseconds int4,
  updated_timestamp timestamp,
  deleted_seconds int8,
  deleted_nanoseconds int4,
  deleted_timestamp timestamp,
  owner_id uuid,
  currency varchar(255),
  name varchar(255),
  balance_signed decimal(19,5),
  balance_absolute decimal(19,5),
  number varchar(255),
  iban varchar(255),
  customer_id varchar(255),
  comment varchar(255),
  bank_official_id int8,
  global_id uuid,
  sync_forced boolean
);


CREATE TABLE bank_official (
  id BIGINT PRIMARY KEY DEFAULT nextval('id'),
  created_seconds int8,
  created_nanoseconds int4,
  created_timestamp timestamp,
  updated_seconds int8,
  updated_nanoseconds int4,
  updated_timestamp timestamp,
  deleted_seconds int8,
  deleted_nanoseconds int4,
  deleted_timestamp timestamp,
  name varchar(255),
  bic varchar(255),
  logo_id int8,
  global_id uuid,
  sync_forced boolean
);


CREATE TABLE file (
  id BIGINT PRIMARY KEY DEFAULT nextval('id'),
  created_seconds int8,
  created_nanoseconds int4,
  created_timestamp timestamp,
  updated_seconds int8,
  updated_nanoseconds int4,
  updated_timestamp timestamp,
  deleted_seconds int8,
  deleted_nanoseconds int4,
  deleted_timestamp timestamp,
  path varchar(255),
  global_id uuid,
  sync_forced boolean
);


ALTER TABLE bank_account add CONSTRAINT FK_98A5B8ED FOREIGN key (bank_official_id) REFERENCES bank_official;


ALTER TABLE bank_official add CONSTRAINT FK_CDDDE635 FOREIGN key (logo_id) REFERENCES file;