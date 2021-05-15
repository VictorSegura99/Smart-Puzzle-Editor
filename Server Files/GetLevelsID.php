<?php

$conn = mysqli_connect("localhost", "id16796074_levelsusers", "6sIHl9OtNu@Wo%~]", "id16796074_levels");

if (!$conn)
{
    echo "Not connected...";
}
else
{
    $sql = "SELECT id FROM Levels";

    $result = $conn->query($sql);

    if ($result->num_rows > 0)
    {
        while ($row = $result->fetch_assoc())
        {
            echo $row["id"]. ",";
        }
    }
    else
    {
        echo "0 results";
    }

    $conn->close();
}

?>