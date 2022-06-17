IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'Anexo_Insert')
BEGIN
	DROP PROCEDURE [dbo].[Anexo_Insert]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Anexo_Insert]
	@ID_Usuario INT = NULL,
	@ID_ClientePloomes INT = NULL,
	@ID_TipoItem INT,
	@ID_Item INT = NULL,
	@Arquivo NVARCHAR(250),
	@Tipo NVARCHAR(100) = NULL,
	@Bytes INT = NULL
AS
BEGIN
	DECLARE @ID_UsuarioVisualizador INT
	IF @ID_Usuario IS NULL BEGIN
		SELECT @ID_UsuarioVisualizador = ID_Criador FROM Ploomes_Cliente WHERE ID = @ID_ClientePloomes
		SET @ID_Usuario = -1
	END
	ELSE BEGIN
		SET @ID_UsuarioVisualizador = @ID_Usuario
		SELECT @ID_ClientePloomes = ID_ClientePloomes FROM Usuario WHERE ID = @ID_Usuario
	END

	IF @ID_Item IS NULL OR @ID_Item = 0 BEGIN
		INSERT INTO Anexo (ID_Item, ID_TipoItem, Arquivo, Tipo, Bytes, ID_Criador) 
				SELECT 0, T.ID, @Arquivo, @Tipo, @Bytes, @ID_Usuario 
				FROM Anexo_TipoItem T 
				WHERE T.ID = @ID_TipoItem
		SELECT SCOPE_IDENTITY() as ID_Anexo
	END
	ELSE BEGIN
		IF @ID_TipoItem = 1 BEGIN
			INSERT INTO Anexo (ID_TipoItem, ID_Item, Arquivo, Tipo, Bytes, ID_Criador)
				SELECT 1, C.ID, @Arquivo, @Tipo, @Bytes, @ID_Usuario
				FROM Vw_Cliente C
				WHERE C.ID_Usuario = @ID_UsuarioVisualizador AND C.ID = @ID_Item AND C.Suspenso = 'False' AND C.Edita = 'True'
		END
		ELSE IF @ID_TipoItem = 2 BEGIN
			INSERT INTO Anexo (ID_TipoItem, ID_Item, Arquivo, Tipo, Bytes, ID_Criador)
				SELECT 2, O.ID, @Arquivo, @Tipo, @Bytes, @ID_Usuario
				FROM Vw_Oportunidade O
				WHERE O.ID_Usuario = @ID_UsuarioVisualizador AND O.ID = @ID_Item AND O.Suspenso = 'False' AND O.Edita = 'True'
		END
		ELSE IF @ID_TipoItem = 3 BEGIN
			INSERT INTO Anexo (ID_TipoItem, ID_Item, Arquivo, Tipo, Bytes, ID_Criador)
				SELECT 3, V.ID, @Arquivo, @Tipo, @Bytes, @ID_Usuario
				FROM Vw_Venda V
				WHERE V.ID_Usuario = @ID_UsuarioVisualizador AND V.ID = @ID_Item AND V.Suspenso = 'False' AND V.Edita = 'True'
		END
		ELSE IF @ID_TipoItem = 4 BEGIN
			INSERT INTO Anexo (ID_TipoItem, ID_Item, Arquivo, Tipo, Bytes, ID_Criador)
				SELECT 4, N.ID, @Arquivo, @Tipo, @Bytes, @ID_Usuario
				FROM Vw_Nota N
				WHERE N.ID_Usuario = @ID_UsuarioVisualizador AND N.ID = @ID_Item AND N.Suspenso = 'False' AND N.Edita = 'True'
		END
		--ELSE IF @ID_TipoItem = 5 BEGIN
		--	INSERT INTO Anexo (ID_TipoItem, ID_Item, Arquivo, Tipo, Bytes, ID_Criador)
		--		SELECT 5, V.ID, @Arquivo, @Tipo, @Bytes, @ID_Usuario
		--		FROM Visita V INNER JOIN Vw_Cliente C ON V.ID_Cliente = C.ID
		--			LEFT JOIN Vw_Oportunidade O ON V.ID_Oportunidade = O.ID
		--		WHERE C.ID_Usuario = @ID_Usuario AND V.ID = @ID_Item
		--			AND (O.ID_Usuario = @ID_Usuario OR V.ID_Oportunidade IS NULL) AND V.Suspenso = 'False'
		--END
		ELSE IF @ID_TipoItem = 6 BEGIN
			INSERT INTO Anexo (ID_TipoItem, ID_Item, Arquivo, Tipo, Bytes, ID_Criador)
				SELECT 6, R.ID, @Arquivo, @Tipo, @Bytes, @ID_Usuario
				FROM Vw_Relatorio2 R1 INNER JOIN SVw_Relatorio R ON R1.ID = R.ID AND R1.ID_Usuario = R.ID_Usuario
				WHERE R.ID_Usuario = @ID_UsuarioVisualizador AND R.ID = @ID_Item AND R.Suspenso = 'False' AND R.Edita = 'True'
		END
		ELSE IF @ID_TipoItem = 8 BEGIN
			INSERT INTO Anexo (ID_TipoItem, ID_Item, Arquivo, Tipo, Bytes, ID_Criador)
				SELECT 1, L.ID, @Arquivo, @Tipo, @Bytes, @ID_Usuario
				FROM Vw_Lead L
				WHERE L.ID_Usuario = @ID_UsuarioVisualizador AND L.ID = @ID_Item AND L.Suspenso = 'False' AND L.Edita = 'True'
		END
		SELECT SCOPE_IDENTITY() as ID_Anexo
	END
END
GO
