namespace MechWars.PlayerInput
{
    public enum CursorType
    {
        Default = 0,
        Select = 1,
        Attack = 4,
        Escort = 2,
        Harvest = 3

        // liczby wg priorytetu, jesli np.
        // kilka jednostek zaznaczonych i harv nie moze atakowac ale moze poharvestowac,
        // ale sa też mechy, to bedzie kursor czerwony mial priorytet nad zoltym
    }
}