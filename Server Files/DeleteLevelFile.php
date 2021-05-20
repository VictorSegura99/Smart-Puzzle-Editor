<?php

$id = $_POST["id"];

$conn = mysqli_connect("localhost", "id16796074_levelsusers", "6sIHl9OtNu@Wo%~]", "id16796074_levels");

if (!$conn)
{
    echo "Error: Not connected...";
}
else
{
    if (!unlink("./levels/$id"))
    {
        echo "Error: could remove file.";
    }

    $conn->close();
}

?>