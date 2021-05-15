<?php

$conn = mysqli_connect("localhost", "id16796074_useradmin", "pn5TGV{vzjQ{lds&", "id16796074_users");

$loginUser = $_POST["username"];
$loginPass = $_POST["Password"];

if (!$conn)
{
    echo "Error: Not connected...";
}
else
{
    $sql = "SELECT Password FROM Users WHERE username = '" . $loginUser . "'";

    $result = $conn->query($sql);

    if ($result->num_rows > 0)
    {
        while ($row = $result->fetch_assoc())
        {
            if ($row["Password"] == $loginPass)
            {
                echo "Login Success.";
            }
            else
            {
                echo "Error: Wrong Credentials";
            }
        }
    }
    else
    {
        echo "Error: Username doesn't exist.";
    }

    $conn->close();
}

?>