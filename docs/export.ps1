$DIR_DRAWIO = "."

$DrawIoFiles = Get-ChildItem $DIR_DRAWIO *.drawio -File

foreach ($file in $DrawIoFiles) {
    
    "File: '$($file.FullName)'"
    
    $xml_file = "$($file.DirectoryName)/$($file.BaseName).xml"

    if ((Test-Path $xml_file)) {
        
        Remove-Item -Path $xml_file -Force
    }
    
    # export to XML
    & "C:/Program Files/draw.io/draw.io.exe" '--export' '--format' 'xml' $file.FullName

    # wait for XML file creation
    while ($true) {
        if (-not (Test-Path $xml_file)) {
            Start-Sleep -Milliseconds 200
        }
        else {
            break
        }
    }
    # load to XML Document (cast text array to object)
    $drawio_xml = [xml](Get-Content $xml_file)

    # for each page export png
    for ($i = 0; $i -lt $drawio_xml.mxfile.pages; $i++) {
        
        $file_out = "$($file.DirectoryName)/$($drawio_xml.mxfile.diagram[$i].name).png"

        & "C:/Program Files/draw.io/draw.io.exe" '--export' '--border' '10' '--page-index' $i '--output' $file_out $file.FullName
    }

    # wait for last file PNG image file
    while ($true) {
        if (-not (Test-Path "$($file.DirectoryName)/$($drawio_xml.mxfile.diagram[$drawio_xml.mxfile.pages - 1].name).png")) {
            Start-Sleep -Milliseconds 200
        }
        else {
            break
        }
    }
    # remove/delete XML file
    if ((Test-Path $xml_file)) {
        
        Remove-Item -Path $xml_file -Force
    }

    # export 'vsdx' & 'pdf'
    #& "C:/Program Files/draw.io/draw.io.exe" '--export' '--format' 'vsdx' $file.FullName
    #& "C:/Program Files/draw.io/draw.io.exe" '--export' '--format' 'pdf' $file.FullName
}

Exit