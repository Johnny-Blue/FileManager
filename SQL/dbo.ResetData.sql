SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Lucian Gadau
-- Create date: Sept 17, 2012
-- Description:	wipes out directory and file tables
-- =============================================
CREATE PROCEDURE dbo.ResetData
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    TRUNCATE TABLE [dbo].[File]
	DELETE FROM [dbo].[Directory]
	DBCC CHECKIDENT ([dbo.Directory], RESEED , 1)
END
GO
