namespace gerenciadorConsultasPICS.Areas.Admin.ViewModels.Instituicao
{
    public class InstituicaoViewModel
    {
        public int? idInstituicao { get; set; }
        public string? nome { get; set; }
        public string? descricao { get; set; }
        public short? idEstado { get; set; }
        public int? idCidade { get; set; }
        public string? cnpj { get; set; }
        public string? cep { get; set; }
        public string? email { get; set; }
        public TimeSpan? horarioInicioAtendimento { get; set; }
        public TimeSpan? horarioFimAtendimento { get; set; }
    }
}
