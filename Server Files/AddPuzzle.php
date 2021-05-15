<?php

$conn = mysqli_connect("localhost", "id16796074_levelsusers", "6sIHl9OtNu@Wo%~]", "id16796074_levels");

$levelID = $_POST["id"];
$levelName = $_POST["levelName"];
$levelDescription = $_POST["levelDescription"];
$levelLikes = $_POST["likes"];
$creatorName = $_POST["creatorName"];

if (!$conn)
{
    echo "Error: Not connected...";
}
else
{
    $sql = "SELECT id FROM Levels WHERE id = '" . $levelID . "'";
    $result = $conn->query($sql);

    if ($result->num_rows > 0)
    {
        echo "Error: Id already existing";
    }
    else
    {
        $sql2 = "INSERT INTO Levels (id, name, description, likes, creatorName) VALUES ('" . $levelID . "','" . $levelName . "', '" . $levelDescription . "', '" . $levelLikes . "', '" . $creatorName . "')";
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