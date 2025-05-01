
CREATE DATABASE IF NOT EXISTS mydb;
CREATE DATABASE IF NOT EXISTS TestDB;


CREATE TABLE IF NOT EXISTS mydb.approved_highvalue_payments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id VARCHAR(50),
    cost DECIMAL(10,2),
    status VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS mydb.customers (
    account_id VARCHAR(50) NOT NULL PRIMARY KEY,
    full_name VARCHAR(100),
    risk_flag VARCHAR(20)
);

CREATE TABLE IF NOT EXISTS mydb.high_risk_accounts (
    account_id VARCHAR(50) NOT NULL PRIMARY KEY,
    risk_flag VARCHAR(20)
);

CREATE TABLE IF NOT EXISTS TestDB.approved_highvalue_payments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id VARCHAR(50),
    cost DECIMAL(10,2),
    status VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS TestDB.customers (
    account_id VARCHAR(50) NOT NULL PRIMARY KEY,
    full_name VARCHAR(100),
    risk_flag VARCHAR(20)
);