
USE dbAgendaPics;

INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Acre', 'AC');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Alagoas', 'AL');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Amapá', 'AP');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Amazonas', 'AM');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Bahia', 'BA');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Ceará', 'CE');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Distrito Federal', 'DF');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Espírito Santo', 'ES');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Goiás', 'GO');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Maranhão', 'MA');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Mato Grosso', 'MT');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Mato Grosso do Sul', 'MS');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Minas Gerais', 'MG');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Pará', 'PA');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Paraíba', 'PB');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Paraná', 'PR');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Pernambuco', 'PE');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Piauí', 'PI');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Rio de Janeiro', 'RJ');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Rio Grande do Norte', 'RN');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Rio Grande do Sul', 'RS');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Rondônia', 'RO');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Roraima', 'RR');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Santa Catarina', 'SC');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('São Paulo', 'SP');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Sergipe', 'SE');
INSERT INTO [dbo].[Estado] ([nome], [sigla]) VALUES ('Tocantins', 'TO');

INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (1, 'Rio Branco'); -- Acre
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (1, 'Cruzeiro do Sul'); -- Acre
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (2, 'Maceió'); -- Alagoas
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (2, 'Arapiraca'); -- Alagoas
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (3, 'Macapá'); -- Amapá
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (3, 'Santana'); -- Amapá
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (4, 'Manaus'); -- Amazonas
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (4, 'Parintins'); -- Amazonas
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (5, 'Salvador'); -- Bahia
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (5, 'Feira de Santana'); -- Bahia
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (6, 'Fortaleza'); -- Ceará
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (6, 'Juazeiro do Norte'); -- Ceará
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (13, 'Uberlândia'); -- Minas Gerais
INSERT INTO [dbo].[Cidade] ([idEstado], [nome]) VALUES (13, 'Monte Carmelo'); -- Minas Gerais

INSERT INTO Pratica
(
nome,
descricao
)
VALUES
('Reiki', 'Técnica energética japonesa que promove equilíbrio e bem-estar por meio da imposição de mãos.'),
('Homeopatia', 'Terapia natural que estimula a cura do corpo com doses diluídas de substâncias naturais.')

INSERT INTO Perfil (descricao)
VALUES('Admin'), ('Instituição')

INSERT INTO Usuario (idPerfil, idInstituicao, login, senha, flPrimeiroAcesso)
VALUES
(1, NULL, 'admin', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 0)
