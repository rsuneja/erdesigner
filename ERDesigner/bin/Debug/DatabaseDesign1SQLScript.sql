Use Master
GO
If EXISTS (Select NAME From SYSDATABASES Where NAME ='DatabaseDesign1')
	Drop database DatabaseDesign1
GO

Create database [DatabaseDesign1]
GO
Use [DatabaseDesign1]
GO

/*==================================================*/ 
/*Table: [ENTITY_0]                                 */ 
/*==================================================*/ 
If EXISTS (Select NAME From SYSOBJECTS Where NAME ='ENTITY_0')
	Drop table [ENTITY_0]
GO
Create table [ENTITY_0]
(
	Attribute_0 varchar(50)  not null
)
GO

Alter table [ENTITY_0]
   add constraint PK_ENTITY_0 primary key (Attribute_0)
GO

/*==============================================================*/
/* Foreign Keys                                                 */
/*==============================================================*/
