static class Validator
{
    public static void ValidateEntity(string entity)
    {
        string[] supportedEntities = new string[] {"movie", "actor", "review"};
        for(int i = 0; i < supportedEntities.Length; i++)
        {
            if(supportedEntities[i] == entity)
            {
                return;
            }
        }
        throw new System.Exception($"Not supported entity {entity}.");
    }
}