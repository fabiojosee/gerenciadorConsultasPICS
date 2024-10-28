using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Areas.Admin.ViewModels.Pratica;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Repositories
{
    public class PraticaInstituicaoRepository : Repository<PraticaInstituicao>, IPraticaInstituicaoRepository
    {
        public PraticaInstituicaoRepository(AppDbContext context) : base(context) { }

        public async Task<PraticaInstituicao> ObterPorPraticaInstituicao(int idInstituicao, short idPratica)
        {
            return await _context.PraticaInstituicao.Where(x => x.idInstituicao == idInstituicao && x.idPratica == idPratica).FirstAsync();
        }

        public async Task<IEnumerable<MinhasPraticasViewModel>> ObterMinhasPraticas(int idInstituicao)
        {
            var praticas = await (from praticaInstituicao in _context.PraticaInstituicao
                                  join pratica in _context.Pratica
                                  on praticaInstituicao.idPratica equals pratica.idPratica
                                  where praticaInstituicao.idInstituicao == idInstituicao
                                  orderby pratica.nome ascending
                                  select new MinhasPraticasViewModel
                                  {
                                      idPratica = praticaInstituicao.idPratica,
                                      idInstituicao = praticaInstituicao.idInstituicao,
                                      nome = pratica.nome,
                                      periodicidade = praticaInstituicao.periodicidade,
                                      qtdSessoes = praticaInstituicao.qtdSessoes,
                                      diaPermitidoParaAgendamento = praticaInstituicao.diaPermitidoParaAgendamento
                                  }).ToListAsync();

            Parallel.ForEach(praticas, pratica =>
            {
                pratica.textoPeriodicidade = $"Uma sessão por {ConverterPeriodicidade(pratica.periodicidade)}.";
                pratica.textoQtdSessoes = $"Total de sessões: {pratica.qtdSessoes}.";
                pratica.textoDiaPermitidoParaAgendamento = $"Dia da semana: {ConverterDiaDaSemana(pratica.diaPermitidoParaAgendamento)}";
            });

            return praticas;
        }

        public async Task<IEnumerable<PraticaInstituicao>> ObterInstituicoesVinculadas(short idPratica)
        {
            return await _context.PraticaInstituicao.Where(x => x.idPratica == idPratica).ToListAsync();
        }

        #region Métodos auxiliares

        private string ConverterPeriodicidade(byte periodicidade)
        {
            string textoPeriodicidade;
            switch (periodicidade)
            {
                case 1:
                    textoPeriodicidade = "dia";
                    break;
                case 2:
                    textoPeriodicidade = "semana";
                    break;
                case 3:
                    textoPeriodicidade = "mês";
                    break;
                default:
                    textoPeriodicidade = "";
                    break;
            }
            return textoPeriodicidade;
        }

        private string ConverterDiaDaSemana(byte diaPermitidoParaAgendamento)
        {
            string textoDiaPermitidoParaAgendamento;
            switch (diaPermitidoParaAgendamento)
            {
                case 0:
                    textoDiaPermitidoParaAgendamento = "Domingo";
                    break;
                case 1:
                    textoDiaPermitidoParaAgendamento = "Segunda-feira";
                    break;
                case 2:
                    textoDiaPermitidoParaAgendamento = "Terça-feira";
                    break;
                case 3:
                    textoDiaPermitidoParaAgendamento = "Quarta-feira";
                    break;
                case 4:
                    textoDiaPermitidoParaAgendamento = "Quinta-feira";
                    break;
                case 5:
                    textoDiaPermitidoParaAgendamento = "Sexta-feira";
                    break;
                case 6:
                    textoDiaPermitidoParaAgendamento = "Sábado";
                    break;
                default:
                    textoDiaPermitidoParaAgendamento = "";
                    break;
            }
            return textoDiaPermitidoParaAgendamento;
        }

        #endregion
    }
}
