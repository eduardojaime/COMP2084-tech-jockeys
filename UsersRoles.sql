-- Insert these two records in the roles table
INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES ('1', 'Administrator', 'Administrator');
INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES ('2', 'Customer', 'Customer');
-- Confirm
SELECT * FROM AspNetRoles; -- 1 Admin, 2 Customer

-- Retrieve Ids and verify users assigned to roles
SELECT * FROM AspNetUsers;
SELECT * FROM AspNetUserRoles;

-- I have three users in my db currently, this will update their roles
-- Modify the ids accordingly below, query AspNetUsers to get your users' ids
INSERT INTO AspNetUserRoles (RoleId, UserId) VALUES
(1, '31c6122f-7820-4810-a67e-df9dd6f08708'),
(2, '8614dc24-07fe-464a-a8d7-c06201379e59'),
(2, 'de5cef06-a402-4444-baa6-cac885ca9e94')