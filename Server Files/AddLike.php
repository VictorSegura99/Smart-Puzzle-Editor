<?php

$conn = mysqli_connect("localhost", "id16796074_levelsusers", "6sIHl9OtNu@Wo%~]", "id16796074_levels");

$levelID = $_POST["id"];
$likeChange = $_POST["likeChange"];
$usersList = $_POST["usersList"];
$currentlikes = -1;

if (!$conn)
{
    echo "Error: Not connected...";
}
else
{
    $sql = "SELECT * FROM Levels WHERE id = '" . $levelID . "'";
    $result = $conn->query($sql);

    if ($result->num_rows < 1)
    {
        echo "Error: Level doesn't exist";
    }
    else
    {
        $row = $result->fetch_assoc();
        $currentlikes = $row["likes"] + $likeChange;

        $sql2 = "UPDATE Levels SET likes = '" . $currentlikes . "' WHERE id = '" . $levelID . "'";
        $result2 = $conn->query($sql2);

        $sql3 = "UPDATE Levels SET usersLiked =  '" . $usersList . "' WHERE id = '" . $levelID . "'";
        $result3 = $conn->query($sql3);
        
        echo $currentlikes;
    }
    

    $conn->close();
}

?>