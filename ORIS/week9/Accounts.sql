﻿CREATE TABLE Accounts (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [Login]    VARCHAR (100) NOT NULL,
    [Password] VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
