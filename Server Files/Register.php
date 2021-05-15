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
    $sql = "SELECT username FROM Users WHERE username = '" . $loginUser . "'";
    $result = $conn->query($sql);

    if ($result->num_rows > 0)
    {
        echo "Error: Username is already taken";
    }
    else
    {
        $sql2 = "INSERT INTO Users (username, Password) VALUES ('" . $loginUser . "','" . $loginPass . "')";
        if ($conn->query($sql2) === TRUE)
        {
            echo "Account created successfully";
        }
        else
        {
            echo "Error: " . $sql2 . "<br>" . $conn->error;    
        }
    }
    

    $conn->close();
}

?>