<?php

$conn = mysqli_connect("localhost", "id16796074_levelsusers", "6sIHl9OtNu@Wo%~]", "id16796074_levels");

if (!$conn)
{
    echo "Not connected...";
}
else
{
    $sql = "SELECT * FROM Levels";

    $result = $conn->query($sql);

    if ($result->num_rows > 0)
    {
        echo $result->num_rows . "/";
        while ($row = $result->fetch_assoc())
        {
            echo $row["id"] . "|" . $row["name"] . "|" . $row["description"] . "|" . $row["likes"]. "|" . $row["creatorName"] . "|" . $row["size"] . "|" . $row["usersLiked"] . "|" .  $row["comments"] . "/";
        }
    }
    else
    {
        echo "0 results";
    }

    $conn->close();
}

?>