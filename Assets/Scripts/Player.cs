public class Player {
    private static Player DUMMY_PLAYER;
    public string name;

    public Player(string name) {
        this.name = name;
    }

    public static Player GET_DUMMY_PLAYER() {
        if (DUMMY_PLAYER == null) {
            DUMMY_PLAYER = new Player("dummy");
        }

        return DUMMY_PLAYER;
    }
}