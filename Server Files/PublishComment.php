<?php

$conn = mysqli_connect("localhost", "id16796074_levelsusers", "6sIHl9OtNu@Wo%~]", "id16796074_levels");

$levelID = $_POST["id"];
$username = $_POST["username"];
$comment = $_POST["comment"];

if (!$conn)
{
    echo "Error: Not connected...";
}
else
{
    $sql = "SELECT comments FROM Levels WHERE id = '" . $levelID . "'";
    $result = $conn->query($sql);

    if ($result->num_rows < 1)
    {
        echo "Error: Id already existing";
    }
    else
    {
        $row = $result->fetch_assoc();
        $comments = $row["comments"] . "|" . $comment . "ª" . $username;

        $sql2 = "UPDATE Levels SET comments = '" . $comments . "' WHERE id = '" . $levelID . "'";
        if ($conn->query($sql2) === TRUE)
        {
            echo "Level added successfully";
        }
        else
        {
            echo "Error: " . $sql2 . "<br>" . $conn->error;    
        }
    }
    

    $conn->close();
}

?>