CREATE TABLE IF NOT EXISTS products(
id INT PRIMARY KEY,
name VARCHAR(256) NOT NULL,
price DECIMAL(13, 4) NOT NULL,
weight DECIMAL(4, 3)
);