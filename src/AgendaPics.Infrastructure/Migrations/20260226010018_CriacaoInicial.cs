using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgendaPics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estado",
                columns: table => new
                {
                    idEstado = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sigla = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estado", x => x.idEstado);
                });

            migrationBuilder.CreateTable(
                name: "Pratica",
                columns: table => new
                {
                    idPratica = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pratica", x => x.idPratica);
                });

            migrationBuilder.CreateTable(
                name: "TokenAcesso",
                columns: table => new
                {
                    idTokenAcesso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cpf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    utilizado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenAcesso", x => x.idTokenAcesso);
                });

            migrationBuilder.CreateTable(
                name: "Cidade",
                columns: table => new
                {
                    idCidade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idEstado = table.Column<short>(type: "smallint", nullable: false),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cidade", x => x.idCidade);
                    table.ForeignKey(
                        name: "FK_Cidade_Estado_idEstado",
                        column: x => x.idEstado,
                        principalTable: "Estado",
                        principalColumn: "idEstado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instituicao",
                columns: table => new
                {
                    idInstituicao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    idCidade = table.Column<int>(type: "int", nullable: false),
                    cnpj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    horarioInicioAtendimento = table.Column<TimeSpan>(type: "time", nullable: false),
                    horarioFimAtendimento = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instituicao", x => x.idInstituicao);
                    table.ForeignKey(
                        name: "FK_Instituicao_Cidade_idCidade",
                        column: x => x.idCidade,
                        principalTable: "Cidade",
                        principalColumn: "idCidade",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agendamento",
                columns: table => new
                {
                    idAgendamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idInstituicao = table.Column<int>(type: "int", nullable: false),
                    idPratica = table.Column<short>(type: "smallint", nullable: false),
                    dataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<byte>(type: "tinyint", nullable: false),
                    observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nomePaciente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cpfPaciente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telefonePaciente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dataNascimentoPaciente = table.Column<DateTime>(type: "datetime2", nullable: false),
                    generoPaciente = table.Column<byte>(type: "tinyint", nullable: false),
                    emailPaciente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    idEstadoPaciente = table.Column<short>(type: "smallint", nullable: false),
                    idCidadePaciente = table.Column<int>(type: "int", nullable: false),
                    grauAnsiedadePaciente = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamento", x => x.idAgendamento);
                    table.ForeignKey(
                        name: "FK_Agendamento_Instituicao_idInstituicao",
                        column: x => x.idInstituicao,
                        principalTable: "Instituicao",
                        principalColumn: "idInstituicao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamento_Pratica_idPratica",
                        column: x => x.idPratica,
                        principalTable: "Pratica",
                        principalColumn: "idPratica",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PraticaInstituicao",
                columns: table => new
                {
                    idPratica = table.Column<short>(type: "smallint", nullable: false),
                    idInstituicao = table.Column<int>(type: "int", nullable: false),
                    periodicidade = table.Column<byte>(type: "tinyint", nullable: false),
                    qtdSessoes = table.Column<short>(type: "smallint", nullable: false),
                    diaPermitidoParaAgendamento = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PraticaInstituicao", x => new { x.idPratica, x.idInstituicao });
                    table.ForeignKey(
                        name: "FK_PraticaInstituicao_Instituicao_idInstituicao",
                        column: x => x.idInstituicao,
                        principalTable: "Instituicao",
                        principalColumn: "idInstituicao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PraticaInstituicao_Pratica_idPratica",
                        column: x => x.idPratica,
                        principalTable: "Pratica",
                        principalColumn: "idPratica",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    idUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idPerfil = table.Column<byte>(type: "tinyint", nullable: false),
                    idInstituicao = table.Column<int>(type: "int", nullable: true),
                    login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    flPrimeiroAcesso = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.idUsuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Instituicao_idInstituicao",
                        column: x => x.idInstituicao,
                        principalTable: "Instituicao",
                        principalColumn: "idInstituicao");
                });

            migrationBuilder.CreateTable(
                name: "Atendimento",
                columns: table => new
                {
                    idAtendimento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idAgendamento = table.Column<int>(type: "int", nullable: false),
                    dataAtendimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<byte>(type: "tinyint", nullable: false),
                    queixaPaciente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    observacao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atendimento", x => x.idAtendimento);
                    table.ForeignKey(
                        name: "FK_Atendimento_Agendamento_idAgendamento",
                        column: x => x.idAgendamento,
                        principalTable: "Agendamento",
                        principalColumn: "idAgendamento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TermoConsentimento",
                columns: table => new
                {
                    idAgendamento = table.Column<int>(type: "int", nullable: false),
                    dataConsentimento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermoConsentimento", x => x.idAgendamento);
                    table.ForeignKey(
                        name: "FK_TermoConsentimento_Agendamento_idAgendamento",
                        column: x => x.idAgendamento,
                        principalTable: "Agendamento",
                        principalColumn: "idAgendamento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacao",
                columns: table => new
                {
                    idAvaliacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idAtendimento = table.Column<int>(type: "int", nullable: false),
                    data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    observacao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacao", x => x.idAvaliacao);
                    table.ForeignKey(
                        name: "FK_Avaliacao_Atendimento_idAtendimento",
                        column: x => x.idAtendimento,
                        principalTable: "Atendimento",
                        principalColumn: "idAtendimento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Estado",
                columns: new[] { "idEstado", "nome", "sigla" },
                values: new object[,]
                {
                    { (short)11, "Rondônia", "RO" },
                    { (short)12, "Acre", "AC" },
                    { (short)13, "Amazonas", "AM" },
                    { (short)14, "Roraima", "RR" },
                    { (short)15, "Pará", "PA" },
                    { (short)16, "Amapá", "AP" },
                    { (short)17, "Tocantins", "TO" },
                    { (short)21, "Maranhão", "MA" },
                    { (short)22, "Piauí", "PI" },
                    { (short)23, "Ceará", "CE" },
                    { (short)24, "Rio Grande do Norte", "RN" },
                    { (short)25, "Paraíba", "PB" },
                    { (short)26, "Pernambuco", "PE" },
                    { (short)27, "Alagoas", "AL" },
                    { (short)28, "Sergipe", "SE" },
                    { (short)29, "Bahia", "BA" },
                    { (short)31, "Minas Gerais", "MG" },
                    { (short)32, "Espírito Santo", "ES" },
                    { (short)33, "Rio de Janeiro", "RJ" },
                    { (short)35, "São Paulo", "SP" },
                    { (short)41, "Paraná", "PR" },
                    { (short)42, "Santa Catarina", "SC" },
                    { (short)43, "Rio Grande do Sul", "RS" },
                    { (short)50, "Mato Grosso do Sul", "MS" },
                    { (short)51, "Mato Grosso", "MT" },
                    { (short)52, "Goiás", "GO" },
                    { (short)53, "Distrito Federal", "DF" }
                });

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "idUsuario", "flPrimeiroAcesso", "idInstituicao", "idPerfil", "login", "senha" },
                values: new object[] { 1, true, null, (byte)1, "admin", "$2a$12$a0oXFr4B3AbjZk3sTh84Puq70PCBj9cf.iEIEm91aWvvnuLXh2Rfu" });

            migrationBuilder.InsertData(
                table: "Cidade",
                columns: new[] { "idCidade", "idEstado", "nome" },
                values: new object[,]
                {
                    { 1100023, (short)11, "Ariquemes" },
                    { 1100049, (short)11, "Cacoal" },
                    { 1100106, (short)11, "Guajará-Mirim" },
                    { 1100114, (short)11, "Jaru" },
                    { 1100122, (short)11, "Ji-Paraná" },
                    { 1100130, (short)11, "Machadinho d'Oeste" },
                    { 1100189, (short)11, "Pimenta Bueno" },
                    { 1100205, (short)11, "Porto Velho" },
                    { 1100288, (short)11, "Rolim de Moura" },
                    { 1100304, (short)11, "Vilhena" },
                    { 1200104, (short)12, "Brasileia" },
                    { 1200203, (short)12, "Cruzeiro do Sul" },
                    { 1200252, (short)12, "Epitaciolândia" },
                    { 1200302, (short)12, "Feijó" },
                    { 1200385, (short)12, "Plácido de Castro" },
                    { 1200401, (short)12, "Rio Branco" },
                    { 1200450, (short)12, "Senador Guiomard" },
                    { 1200500, (short)12, "Sena Madureira" },
                    { 1200609, (short)12, "Tarauacá" },
                    { 1200708, (short)12, "Xapuri" },
                    { 1301209, (short)13, "Coari" },
                    { 1301704, (short)13, "Humaitá" },
                    { 1301852, (short)13, "Iranduba" },
                    { 1301902, (short)13, "Itacoatiara" },
                    { 1302504, (short)13, "Manacapuru" },
                    { 1302603, (short)13, "Manaus" },
                    { 1302900, (short)13, "Maués" },
                    { 1303403, (short)13, "Parintins" },
                    { 1304062, (short)13, "Tabatinga" },
                    { 1304203, (short)13, "Tefé" },
                    { 1400027, (short)14, "Amajari" },
                    { 1400050, (short)14, "Alto Alegre" },
                    { 1400100, (short)14, "Boa Vista" },
                    { 1400159, (short)14, "Bonfim" },
                    { 1400175, (short)14, "Cantá" },
                    { 1400209, (short)14, "Caracaraí" },
                    { 1400282, (short)14, "Iracema" },
                    { 1400308, (short)14, "Mucajaí" },
                    { 1400456, (short)14, "Pacaraima" },
                    { 1400472, (short)14, "Rorainópolis" },
                    { 1500107, (short)15, "Abaetetuba" },
                    { 1500602, (short)15, "Altamira" },
                    { 1500859, (short)15, "Ananindeua" },
                    { 1501402, (short)15, "Belém" },
                    { 1502103, (short)15, "Cametá" },
                    { 1502400, (short)15, "Castanhal" },
                    { 1503606, (short)15, "Itaituba" },
                    { 1504208, (short)15, "Marabá" },
                    { 1505536, (short)15, "Parauapebas" },
                    { 1507204, (short)15, "Santarém" },
                    { 1600057, (short)16, "Pedra Branca do Amapari" },
                    { 1600204, (short)16, "Calçoene" },
                    { 1600279, (short)16, "Laranjal do Jari" },
                    { 1600303, (short)16, "Macapá" },
                    { 1600402, (short)16, "Mazagão" },
                    { 1600501, (short)16, "Oiapoque" },
                    { 1600535, (short)16, "Porto Grande" },
                    { 1600600, (short)16, "Santana" },
                    { 1600709, (short)16, "Tartarugalzinho" },
                    { 1600808, (short)16, "Vitória do Jari" },
                    { 1702109, (short)17, "Araguaína" },
                    { 1702703, (short)17, "Araguatins" },
                    { 1709302, (short)17, "Guaraí" },
                    { 1709500, (short)17, "Gurupi" },
                    { 1713205, (short)17, "Miracema do Tocantins" },
                    { 1716109, (short)17, "Paraíso do Tocantins" },
                    { 1716802, (short)17, "Colinas do Tocantins" },
                    { 1718204, (short)17, "Porto Nacional" },
                    { 1721000, (short)17, "Palmas" },
                    { 1721109, (short)17, "Tocantinópolis" },
                    { 2100055, (short)21, "Açailândia" },
                    { 2101202, (short)21, "Bacabal" },
                    { 2101400, (short)21, "Balsas" },
                    { 2103000, (short)21, "Caxias" },
                    { 2103307, (short)21, "Codó" },
                    { 2105302, (short)21, "Imperatriz" },
                    { 2107800, (short)21, "Paço do Lumiar" },
                    { 2111201, (short)21, "São José de Ribamar" },
                    { 2111300, (short)21, "São Luís" },
                    { 2112209, (short)21, "Timon" },
                    { 2200400, (short)22, "Altos" },
                    { 2201200, (short)22, "Barras" },
                    { 2202208, (short)22, "Campo Maior" },
                    { 2203909, (short)22, "Floriano" },
                    { 2205508, (short)22, "José de Freitas" },
                    { 2207702, (short)22, "Parnaíba" },
                    { 2208007, (short)22, "Picos" },
                    { 2208304, (short)22, "Piripiri" },
                    { 2211001, (short)22, "Teresina" },
                    { 2211100, (short)22, "União" },
                    { 2303709, (short)23, "Caucaia" },
                    { 2304202, (short)23, "Crato" },
                    { 2304400, (short)23, "Fortaleza" },
                    { 2305506, (short)23, "Iguatu" },
                    { 2306405, (short)23, "Itapipoca" },
                    { 2307304, (short)23, "Juazeiro do Norte" },
                    { 2307650, (short)23, "Maracanaú" },
                    { 2307700, (short)23, "Maranguape" },
                    { 2311306, (short)23, "Quixadá" },
                    { 2312908, (short)23, "Sobral" },
                    { 2400109, (short)24, "Açu" },
                    { 2402006, (short)24, "Caicó" },
                    { 2402600, (short)24, "Ceará-Mirim" },
                    { 2403103, (short)24, "Currais Novos" },
                    { 2403251, (short)24, "Parnamirim" },
                    { 2408003, (short)24, "Mossoró" },
                    { 2408102, (short)24, "Natal" },
                    { 2411403, (short)24, "Santa Cruz" },
                    { 2412005, (short)24, "São Gonçalo do Amarante" },
                    { 2412203, (short)24, "São José de Mipibu" },
                    { 2502003, (short)25, "Bayeux" },
                    { 2503209, (short)25, "Cabedelo" },
                    { 2503704, (short)25, "Cajazeiras" },
                    { 2504009, (short)25, "Campina Grande" },
                    { 2505501, (short)25, "Guarabira" },
                    { 2507507, (short)25, "João Pessoa" },
                    { 2510808, (short)25, "Patos" },
                    { 2513703, (short)25, "Santa Rita" },
                    { 2515302, (short)25, "Sapé" },
                    { 2516201, (short)25, "Sousa" },
                    { 2604106, (short)26, "Caruaru" },
                    { 2606002, (short)26, "Garanhuns" },
                    { 2606804, (short)26, "Igarassu" },
                    { 2607901, (short)26, "Jaboatão dos Guararapes" },
                    { 2609600, (short)26, "Olinda" },
                    { 2610707, (short)26, "Paulista" },
                    { 2611101, (short)26, "Petrolina" },
                    { 2611606, (short)26, "Recife" },
                    { 2612208, (short)26, "Ribeirão" },
                    { 2616407, (short)26, "Vitória de Santo Antão" },
                    { 2700300, (short)27, "Arapiraca" },
                    { 2702306, (short)27, "Coruripe" },
                    { 2702405, (short)27, "Delmiro Gouveia" },
                    { 2704302, (short)27, "Maceió" },
                    { 2706101, (short)27, "Palmeira dos Índios" },
                    { 2706703, (short)27, "Penedo" },
                    { 2707701, (short)27, "Rio Largo" },
                    { 2708303, (short)27, "Santana do Ipanema" },
                    { 2708600, (short)27, "São Miguel dos Campos" },
                    { 2709103, (short)27, "União dos Palmares" },
                    { 2800308, (short)28, "Aracaju" },
                    { 2802106, (short)28, "Estância" },
                    { 2802908, (short)28, "Itabaiana" },
                    { 2803500, (short)28, "Lagarto" },
                    { 2804409, (short)28, "Nossa Senhora das Dores" },
                    { 2804508, (short)28, "Nossa Senhora do Socorro" },
                    { 2805406, (short)28, "Propriá" },
                    { 2806701, (short)28, "São Cristóvão" },
                    { 2807105, (short)28, "Simão Dias" },
                    { 2807402, (short)28, "Tobias Barreto" },
                    { 2905701, (short)29, "Camaçari" },
                    { 2910800, (short)29, "Feira de Santana" },
                    { 2913606, (short)29, "Ilhéus" },
                    { 2914802, (short)29, "Itabuna" },
                    { 2917903, (short)29, "Jequié" },
                    { 2918407, (short)29, "Juazeiro" },
                    { 2919207, (short)29, "Lauro de Freitas" },
                    { 2927408, (short)29, "Salvador" },
                    { 2931350, (short)29, "Teixeira de Freitas" },
                    { 2933307, (short)29, "Vitória da Conquista" },
                    { 3106200, (short)31, "Belo Horizonte" },
                    { 3106705, (short)31, "Betim" },
                    { 3118601, (short)31, "Contagem" },
                    { 3127701, (short)31, "Governador Valadares" },
                    { 3131307, (short)31, "Ipatinga" },
                    { 3136702, (short)31, "Juiz de Fora" },
                    { 3143104, (short)31, "Monte Carmelo" },
                    { 3143302, (short)31, "Montes Claros" },
                    { 3154606, (short)31, "Ribeirão das Neves" },
                    { 3170107, (short)31, "Uberaba" },
                    { 3170206, (short)31, "Uberlândia" },
                    { 3200607, (short)32, "Aracruz" },
                    { 3201209, (short)32, "Cachoeiro de Itapemirim" },
                    { 3201308, (short)32, "Cariacica" },
                    { 3201506, (short)32, "Colatina" },
                    { 3202405, (short)32, "Guarapari" },
                    { 3203205, (short)32, "Linhares" },
                    { 3204906, (short)32, "São Mateus" },
                    { 3205010, (short)32, "Serra" },
                    { 3205200, (short)32, "Vila Velha" },
                    { 3205309, (short)32, "Vitória" },
                    { 3300456, (short)33, "Belford Roxo" },
                    { 3301009, (short)33, "Campos dos Goytacazes" },
                    { 3301702, (short)33, "Duque de Caxias" },
                    { 3303302, (short)33, "Niterói" },
                    { 3303500, (short)33, "Nova Iguaçu" },
                    { 3303906, (short)33, "Petrópolis" },
                    { 3304557, (short)33, "Rio de Janeiro" },
                    { 3304904, (short)33, "São Gonçalo" },
                    { 3305109, (short)33, "São João de Meriti" },
                    { 3306701, (short)33, "Volta Redonda" },
                    { 3509502, (short)35, "Campinas" },
                    { 3518800, (short)35, "Guarulhos" },
                    { 3529401, (short)35, "Mauá" },
                    { 3534401, (short)35, "Osasco" },
                    { 3543402, (short)35, "Ribeirão Preto" },
                    { 3547809, (short)35, "Santo André" },
                    { 3548708, (short)35, "São Bernardo do Campo" },
                    { 3549904, (short)35, "São José dos Campos" },
                    { 3550308, (short)35, "São Paulo" },
                    { 3552205, (short)35, "Sorocaba" },
                    { 4104808, (short)41, "Cascavel" },
                    { 4105805, (short)41, "Colombo" },
                    { 4106902, (short)41, "Curitiba" },
                    { 4108304, (short)41, "Foz do Iguaçu" },
                    { 4109401, (short)41, "Guarapuava" },
                    { 4113700, (short)41, "Londrina" },
                    { 4115200, (short)41, "Maringá" },
                    { 4118204, (short)41, "Paranaguá" },
                    { 4119905, (short)41, "Ponta Grossa" },
                    { 4125506, (short)41, "São José dos Pinhais" },
                    { 4202008, (short)42, "Balneário Camboriú" },
                    { 4202404, (short)42, "Blumenau" },
                    { 4204202, (short)42, "Chapecó" },
                    { 4204608, (short)42, "Criciúma" },
                    { 4205407, (short)42, "Florianópolis" },
                    { 4208203, (short)42, "Itajaí" },
                    { 4208906, (short)42, "Jaraguá do Sul" },
                    { 4209102, (short)42, "Joinville" },
                    { 4211900, (short)42, "Palhoça" },
                    { 4216602, (short)42, "São José" },
                    { 4304606, (short)43, "Canoas" },
                    { 4305108, (short)43, "Caxias do Sul" },
                    { 4309209, (short)43, "Gravataí" },
                    { 4313409, (short)43, "Novo Hamburgo" },
                    { 4314100, (short)43, "Pelotas" },
                    { 4314902, (short)43, "Porto Alegre" },
                    { 4315602, (short)43, "Rio Grande" },
                    { 4316907, (short)43, "Santa Maria" },
                    { 4318705, (short)43, "São Leopoldo" },
                    { 4323002, (short)43, "Viamão" },
                    { 5001102, (short)50, "Aquidauana" },
                    { 5002704, (short)50, "Campo Grande" },
                    { 5003207, (short)50, "Corumbá" },
                    { 5003702, (short)50, "Dourados" },
                    { 5005904, (short)50, "Naviraí" },
                    { 5006200, (short)50, "Nova Andradina" },
                    { 5006309, (short)50, "Paranaíba" },
                    { 5006606, (short)50, "Ponta Porã" },
                    { 5007901, (short)50, "Sidrolândia" },
                    { 5008305, (short)50, "Três Lagoas" },
                    { 5101803, (short)51, "Barra do Garças" },
                    { 5102504, (short)51, "Cáceres" },
                    { 5103403, (short)51, "Cuiabá" },
                    { 5105259, (short)51, "Lucas do Rio Verde" },
                    { 5106455, (short)51, "Primavera do Leste" },
                    { 5107602, (short)51, "Rondonópolis" },
                    { 5107909, (short)51, "Sinop" },
                    { 5107925, (short)51, "Sorriso" },
                    { 5107941, (short)51, "Tangará da Serra" },
                    { 5108402, (short)51, "Várzea Grande" },
                    { 5200050, (short)52, "Águas Lindas de Goiás" },
                    { 5201108, (short)52, "Anápolis" },
                    { 5201405, (short)52, "Aparecida de Goiânia" },
                    { 5208004, (short)52, "Formosa" },
                    { 5208707, (short)52, "Goiânia" },
                    { 5212501, (short)52, "Luziânia" },
                    { 5215231, (short)52, "Novo Gama" },
                    { 5218805, (short)52, "Rio Verde" },
                    { 5221403, (short)52, "Trindade" },
                    { 5221858, (short)52, "Valparaíso de Goiás" },
                    { 5300108, (short)53, "Brasília" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_idInstituicao",
                table: "Agendamento",
                column: "idInstituicao");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_idPratica",
                table: "Agendamento",
                column: "idPratica");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimento_idAgendamento",
                table: "Atendimento",
                column: "idAgendamento");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_idAtendimento",
                table: "Avaliacao",
                column: "idAtendimento");

            migrationBuilder.CreateIndex(
                name: "IX_Cidade_idEstado",
                table: "Cidade",
                column: "idEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Instituicao_idCidade",
                table: "Instituicao",
                column: "idCidade");

            migrationBuilder.CreateIndex(
                name: "IX_PraticaInstituicao_idInstituicao",
                table: "PraticaInstituicao",
                column: "idInstituicao");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_idInstituicao",
                table: "Usuario",
                column: "idInstituicao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacao");

            migrationBuilder.DropTable(
                name: "PraticaInstituicao");

            migrationBuilder.DropTable(
                name: "TermoConsentimento");

            migrationBuilder.DropTable(
                name: "TokenAcesso");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Atendimento");

            migrationBuilder.DropTable(
                name: "Agendamento");

            migrationBuilder.DropTable(
                name: "Instituicao");

            migrationBuilder.DropTable(
                name: "Pratica");

            migrationBuilder.DropTable(
                name: "Cidade");

            migrationBuilder.DropTable(
                name: "Estado");
        }
    }
}
