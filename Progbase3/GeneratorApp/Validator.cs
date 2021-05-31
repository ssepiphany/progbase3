static class Validator
{
    public static void ValidateEntity(string entity)
    {
        string[] supportedEntities = new string[] {"movie", "actor", "review", "user"};
        for(int i = 0; i < supportedEntities.Length; i++)
        {
            if(supportedEntities[i] == entity)
            {
                return;
            }
        }
        throw new System.Exception($"Not supported entity {entity}.");
    }

    public static void ValidateLength(int length, int desiredLength)
    {
        if(length != desiredLength)
        {
            throw new System.Exception($"Invalid command.");
        }
    }

    public static int ValidateNumber(string number)
    {
        int num;
        if(!int.TryParse(number, out num))
        {
            throw new System.Exception($"Invalid number: {number}.");
        }
        return num;
    }
}