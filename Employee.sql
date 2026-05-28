CREATE TABLE Employee
(
    ID        INT           PRIMARY KEY IDENTITY(1,1),
    Name      VARCHAR(100)  NOT NULL,
    ManagerID INT           NULL,
    Enable    BIT           NOT NULL DEFAULT 1,

    CONSTRAINT FK_Employee_Manager 
        FOREIGN KEY (ManagerID) REFERENCES Employee(ID)
);
GO


INSERT INTO Employee (Name, ManagerID, Enable) VALUES 
('Andrey', NULL, 1),  
('Anna',   1,    1),  
('Sim',    2,    1), 
('Alexey', 1,    1),  
('Barak',  2,    1),  
('Roman',  3,    0),  
('Lena',   3,    1);
GO


  SELECT ID, Name, ManagerID, Enable FROM Employee;
GO