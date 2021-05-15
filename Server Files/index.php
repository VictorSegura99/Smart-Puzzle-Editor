<?php

if (isset($_FILES['myfile']))
{
    $level = $_FILES['myfile']['name'];
    $tmplevel = $_FILES['myfile']['tmp_name'];

    move_uploaded_file($tmplevel, "./levels/$level");

    echo "[success] level ($level) uploaded successfully.";
    exit();
}
else
{
    echo "[error] there is no data with name [myfile]";
}

?>