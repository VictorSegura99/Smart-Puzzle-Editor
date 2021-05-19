<?php

$conn = mysqli_connect("localhost", "id16796074_levelsusers", "6sIHl9OtNu@Wo%~]", "id16796074_levels");

$levelID = $_POST["id"];

if (!$conn)
{
    echo "Error: Not connected...";
}
else
{
    $sql = "SELECT usersLiked FROM Levels WHERE id = '" . $levelID . "'";
    $result = $conn->query($sql);

    if ($result->num_rows < 1)
    {
        echo "Error: Level doesn't exist";
    }
    else
    {
        while ($row = $result->fetch_assoc())
        {
            echo $row["usersLiked"];
        }
    }
    

    $conn->close();
}

?>