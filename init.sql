CREATE DATABASE IF NOT EXISTS mydb;
USE mydb;

CREATE TABLE IF NOT EXISTS approved_highvalue_payments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id VARCHAR(50),
    cost DECIMAL(10,2),
    status VARCHAR(50)
);