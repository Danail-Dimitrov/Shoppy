CREATE TABLE IF NOT EXISTS extendeduser(
Id varchar(255) UNIQUE NOT NULL,
Money DECIMAL(13, 4),
LoyaltyScore SMALLINT,
FOREIGN KEY (Id) REFERENCES aspnetusers(Id)
);

