namespace InteligenteZap.Domain.Shared;

public class PaginatedList<T>
{
    public PaginatedList(List<T> itens, int total, int pagina, int tamanhoDaPagina)
    {
        Itens = itens;
        Pagina = pagina;
        TamanhoDaPagina = tamanhoDaPagina;
        TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoDaPagina);
    }

    public List<T> Itens { get; private set; } = new();
    public int Pagina { get; private set; }
    public int TamanhoDaPagina { get; private set; }
    public int TotalPaginas { get; private set; }

    public bool PossuiPaginaAnterior => Pagina > 1;
    public bool PossuiProximaPagina => Pagina < TotalPaginas;
    public bool EhUltimaPagina => Pagina == TotalPaginas;
}
