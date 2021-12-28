--
-- PostgreSQL database dump
--

-- Dumped from database version 12.0
-- Dumped by pg_dump version 12.0

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'WIN1251';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: directum_test_task_db; Type: DATABASE; Schema: -; Owner: -
--

CREATE DATABASE directum_test_task_db WITH TEMPLATE = template0 ENCODING = 'WIN1251' LC_COLLATE = 'English_United States.1251' LC_CTYPE = 'English_United States.1251';


\connect directum_test_task_db

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'WIN1251';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: DATABASE directum_test_task_db; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON DATABASE directum_test_task_db IS 'База данных тестового задания "Directum"';


--
-- Name: test_task_schema; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA test_task_schema;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: department; Type: TABLE; Schema: test_task_schema; Owner: -
--

CREATE TABLE test_task_schema.department (
    id numeric(11,0) NOT NULL,
    name character varying(100) NOT NULL
);


--
-- Name: TABLE department; Type: COMMENT; Schema: test_task_schema; Owner: -
--

COMMENT ON TABLE test_task_schema.department IS 'Departments';


--
-- Name: employee; Type: TABLE; Schema: test_task_schema; Owner: -
--

CREATE TABLE test_task_schema.employee (
    id numeric(11,0) NOT NULL,
    department_id numeric(11,0) NOT NULL,
    chief_id numeric(11,0),
    name character varying(100) NOT NULL,
    salary numeric(11,0) NOT NULL
);


--
-- Name: TABLE employee; Type: COMMENT; Schema: test_task_schema; Owner: -
--

COMMENT ON TABLE test_task_schema.employee IS 'Employees';


--
-- Data for Name: department; Type: TABLE DATA; Schema: test_task_schema; Owner: -
--

INSERT INTO test_task_schema.department (id, name) VALUES (1, 'D1');
INSERT INTO test_task_schema.department (id, name) VALUES (2, 'D2');
INSERT INTO test_task_schema.department (id, name) VALUES (3, 'D3');


--
-- Data for Name: employee; Type: TABLE DATA; Schema: test_task_schema; Owner: -
--

INSERT INTO test_task_schema.employee (id, department_id, chief_id, name, salary) VALUES (1, 1, 5, 'John', 100);
INSERT INTO test_task_schema.employee (id, department_id, chief_id, name, salary) VALUES (2, 1, 5, 'Misha', 600);
INSERT INTO test_task_schema.employee (id, department_id, chief_id, name, salary) VALUES (3, 2, 6, 'Eugen', 300);
INSERT INTO test_task_schema.employee (id, department_id, chief_id, name, salary) VALUES (4, 2, 6, 'Tolya', 400);
INSERT INTO test_task_schema.employee (id, department_id, chief_id, name, salary) VALUES (5, 3, 7, 'Stepan', 500);
INSERT INTO test_task_schema.employee (id, department_id, chief_id, name, salary) VALUES (6, 3, 7, 'Alex', 1000);
INSERT INTO test_task_schema.employee (id, department_id, chief_id, name, salary) VALUES (7, 3, NULL, 'Ivan', 1100);


--
-- Name: department department_pkey; Type: CONSTRAINT; Schema: test_task_schema; Owner: -
--

ALTER TABLE ONLY test_task_schema.department
    ADD CONSTRAINT department_pkey PRIMARY KEY (id);


--
-- Name: employee employee_pkey; Type: CONSTRAINT; Schema: test_task_schema; Owner: -
--

ALTER TABLE ONLY test_task_schema.employee
    ADD CONSTRAINT employee_pkey PRIMARY KEY (id);


--
-- Name: employee chief_link; Type: FK CONSTRAINT; Schema: test_task_schema; Owner: -
--

ALTER TABLE ONLY test_task_schema.employee
    ADD CONSTRAINT chief_link FOREIGN KEY (chief_id) REFERENCES test_task_schema.employee(id) NOT VALID;


--
-- Name: employee department_link; Type: FK CONSTRAINT; Schema: test_task_schema; Owner: -
--

ALTER TABLE ONLY test_task_schema.employee
    ADD CONSTRAINT department_link FOREIGN KEY (department_id) REFERENCES test_task_schema.department(id);


--
-- PostgreSQL database dump complete
--

