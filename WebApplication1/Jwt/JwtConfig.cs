namespace login.jwt.config
{
public class JwtConfig{
    public string? secret{get;set;}
    public TimeSpan ExpiryTimeFrame{get;set;}
}
    
}