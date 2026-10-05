namespace DeskFlow.API.Exceptions;

public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem) { }
}

public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem) { }
}