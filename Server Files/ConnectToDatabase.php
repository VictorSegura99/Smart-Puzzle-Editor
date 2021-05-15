<?php

$conn = mysqli_connect("localhost", "id16796074_useradmin", "pn5TGV{vzjQ{lds&", "id16796074_users");

if (!$conn)
{
    echo "Not connected...";
}
else
{
    echo "Connection Successfully". "<br>";

    $sql = "SELECT username, Password FROM Users";

    $result = $conn->query($sql);

    if ($result->num_rows > 0)
    {
        while ($row = $result->fetch_assoc())
        {
            echo "username: " . $row["username"]. " - Password: " . $row["Password"]. "<br>";
        }
    }
    else
    {
        echo "0 results";
    }

    $conn->close();
}

?>