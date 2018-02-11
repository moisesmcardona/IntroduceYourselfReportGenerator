Imports System.IO
Imports System.Text
Imports System.Web
Imports DSharpPlus
Imports MySql.Data.MySqlClient

Public Class Form1
    Private WithEvents DiscordClient As DiscordClient
    Private DiscordChannelObject As DiscordChannel
    Private WithEvents DiscordClientLogger As DebugLogger

    Dim MySQLString As String = String.Empty

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        RefreshListBox()
    End Sub
    Private Sub RefreshListBox()
        ListBox1.Items.Clear()
        ListBox2.Items.Clear()
        ListBox3.Items.Clear()
        LoadPosts()
        LoadOKPosts()
        LoadFlaggedPosts()
    End Sub
    Private Sub LoadPosts()
        Dim TextBoxDate As DateTime = DateTime.ParseExact(TextBox1.Text, "MM/dd/yyyy", Nothing)
        Dim SQLQuery As String = ""
        If RadioButton1.Checked Then
            SQLQuery = "SELECT DISTINCT link FROM bienvenida WHERE verified=0 AND posted LIKE '" & TextBoxDate.ToString("yyyy/MM/dd") & "%' ORDER BY username ASC"
        Else
            SQLQuery = "SELECT DISTINCT link FROM introduceyourself WHERE verified=0 AND posted LIKE '" & TextBoxDate.ToString("yyyy/MM/dd") & "%' ORDER BY username ASC"
        End If
        Dim Connection As MySqlConnection = New MySqlConnection(MySQLString)
        Dim Command As New MySqlCommand(SQLQuery, Connection)
        Connection.Open()
        Dim reader As MySqlDataReader = Command.ExecuteReader
        If reader.HasRows = True Then
            While reader.Read
                ListBox2.Items.Add(reader("link"))
            End While
        End If
        Connection.Close()
    End Sub
    Private Sub LoadOKPosts()
        Dim TextBoxDate As DateTime = DateTime.ParseExact(TextBox1.Text, "MM/dd/yyyy", Nothing)
        Dim SQLQuery As String = ""
        If RadioButton1.Checked Then
            SQLQuery = "SELECT DISTINCT link FROM bienvenida WHERE verified=1 AND posted LIKE '" & TextBoxDate.ToString("yyyy/MM/dd") & "%' ORDER BY username ASC"
        Else
            SQLQuery = "SELECT DISTINCT link FROM introduceyourself WHERE verified=1 AND posted LIKE '" & TextBoxDate.ToString("yyyy/MM/dd") & "%' ORDER BY username ASC"
        End If
        Dim Connection As MySqlConnection = New MySqlConnection(MySQLString)
        Dim Command As New MySqlCommand(SQLQuery, Connection)
        Connection.Open()
        Dim reader As MySqlDataReader = Command.ExecuteReader
        If reader.HasRows = True Then
            While reader.Read
                ListBox1.Items.Add(reader("link"))
            End While
        End If
        Connection.Close()
    End Sub
    Private Sub LoadFlaggedPosts()
        Dim TextBoxDate As DateTime = DateTime.ParseExact(TextBox1.Text, "MM/dd/yyyy", Nothing)
        Dim SQLQuery As String = ""
        If RadioButton1.Checked Then
            SQLQuery = "SELECT DISTINCT link FROM bienvenida WHERE verified=2 AND posted LIKE '" & TextBoxDate.ToString("yyyy/MM/dd") & "%' ORDER BY username ASC"
        Else
            SQLQuery = "SELECT DISTINCT link FROM introduceyourself WHERE verified=2 AND posted LIKE '" & TextBoxDate.ToString("yyyy/MM/dd") & "%' ORDER BY username ASC"
        End If
        Dim Connection As MySqlConnection = New MySqlConnection(MySQLString)
        Dim Command As New MySqlCommand(SQLQuery, Connection)
        Connection.Open()
        Dim reader As MySqlDataReader = Command.ExecuteReader
        If reader.HasRows = True Then
            While reader.Read
                ListBox3.Items.Add(reader("link"))
            End While
        End If
        Connection.Close()
    End Sub

    Private Sub ListBox1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDoubleClick
        If ListBox1.SelectedIndex >= 0 Then Process.Start("https://steemit.com/tag/" & ListBox1.SelectedItem)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim TextBoxDate As DateTime = DateTime.ParseExact(TextBox1.Text, "MM/dd/yyyy", Nothing)
        If RadioButton1.Checked Then
            Dim SQLQuery As String = "SELECT DISTINCT * FROM bienvenida WHERE verified=1 AND posted LIKE '" & TextBoxDate.ToString("yyyy/MM/dd") & "%' ORDER BY username ASC"
            Dim Connection As MySqlConnection = New MySqlConnection(MySQLString)
            Dim Command As New MySqlCommand(SQLQuery, Connection)
            Connection.Open()
            Dim reader As MySqlDataReader = Command.ExecuteReader
            Dim Profile As String = "", PK As String = "", VP As String = ""
            Dim LogFile As New StreamWriter("report-bienvenida-" & TextBox1.Text.Replace("/", "-") & ".txt", False)
            If reader.HasRows = True Then
                Dim FullDate As String = GetFullDate(TextBoxDate)
                LogFile.WriteLine("<center>![](https://steem.place/images/Bienvenida.PNG)</center>")
                LogFile.WriteLine("Los siguientes usuarios han escrito su introducción a la comunidad o publicado su primer post en español utilizando el tag *spanish* en el día " & FullDate.ToLower & ". La lista está en orden alfabético:" & Environment.NewLine & Environment.NewLine)
                LogFile.WriteLine("Usuario | Post")
                LogFile.WriteLine("------- | ----")
                Dim Link As String = ""
                Dim User As String = ""
                Dim Title As String = ""
                While reader.Read
                    Link = reader("link")
                    User = reader("username")
                    Title = getPostTitle(Link)
                    LogFile.WriteLine("@" & User & " | [" & Title.Replace("|", ":") & "](https://steemit.com/tag/" & Link & ")")
                End While
                LogFile.WriteLine(Environment.NewLine & "---------------------" & Environment.NewLine)
                LogFile.WriteLine("<center>Les recordamos que existen varias iniciativas para ayudar a la comunidad hispana a crecer:</center>" & Environment.NewLine)
                LogFile.WriteLine("<center>[Click aquí para ver la lista de las inciativas](https://steem.place/Iniciativas)</center>")
                LogFile.WriteLine(Environment.NewLine & "---------------------" & Environment.NewLine)
                LogFile.WriteLine("<center>[¡Vota al Witness @castellano!](https://v2.steemconnect.com/sign/account-witness-vote?witness=castellano&approve=1)</center>")
                LogFile.WriteLine(Environment.NewLine & "---------------------" & Environment.NewLine)
                LogFile.WriteLine("<center>Reporte generado por el software de @moisesmcardona. [Vótalo como Witness presionando aquí](https://v2.steemconnect.com/sign/account-witness-vote?witness=moisesmcardona&approve=1)</center>" & Environment.NewLine & Environment.NewLine)
                LogFile.Close()
                PublishReport(FullDate)
                SendMessage(FullDate, TextBox1.Text)
            End If
        Else
            Dim SQLQuery As String = "SELECT DISTINCT * FROM introduceyourself WHERE verified=1 AND posted LIKE '" & TextBoxDate.ToString("yyyy/MM/dd") & "%' ORDER BY username ASC"
            Dim Connection As MySqlConnection = New MySqlConnection(MySQLString)
            Dim Command As New MySqlCommand(SQLQuery, Connection)
            Connection.Open()
            Dim reader As MySqlDataReader = Command.ExecuteReader
            Dim Profile As String = "", PK As String = "", VP As String = ""
            Dim LogFile As New StreamWriter("report-introduceyourself-" & TextBox1.Text.Replace("/", "-") & ".txt", False)
            If reader.HasRows = True Then
                Dim FullDate As String = GetFullDate(TextBoxDate)
                LogFile.WriteLine("<center>![](https://steem.place/images/IntroduceYourselfV2.PNG)</center>")
                LogFile.WriteLine("The following users have made their #introduceyourself post yesterday, " & FullDate & ". The list is in Alphabetical Order:" & Environment.NewLine & Environment.NewLine)
                LogFile.WriteLine("User | Post")
                LogFile.WriteLine("------- | ----")
                Dim Link As String = ""
                Dim User As String = ""
                Dim Title As String = ""
                While reader.Read
                    Link = reader("link")
                    User = reader("username")
                    Title = getPostTitle(Link)
                    LogFile.WriteLine("@" & User & " | [" & Title.Replace("|", ":") & "](https://steemit.com/tag/" & Link & ")")
                End While
                LogFile.WriteLine(Environment.NewLine & "---------------------" & Environment.NewLine)
                LogFile.WriteLine("<center>Report generated by @moisesmcardona's software. Consider voting him as Witness at [https://steemit.com/~witnesses](https://steemit.com/~witnesses)</center>" & Environment.NewLine & Environment.NewLine)
                LogFile.Close()
                PublishReport(FullDate)
            End If
        End If
    End Sub
    Private Function GetFullDate(TextBoxDate As DateTime) As String
        Dim FullDate As String = vbEmpty
        If RadioButton2.Checked = True Then
            Dim Month As String = TextBoxDate.ToString("MM")
            Dim MonthName As String = ""
            If Month = "01" Then
                MonthName = "January"
            ElseIf Month = "02" Then
                MonthName = "February"
            ElseIf Month = "03" Then
                MonthName = "March"
            ElseIf Month = "04" Then
                MonthName = "April"
            ElseIf Month = "05" Then
                MonthName = "May"
            ElseIf Month = "06" Then
                MonthName = "June"
            ElseIf Month = "07" Then
                MonthName = "July"
            ElseIf Month = "08" Then
                MonthName = "August"
            ElseIf Month = "09" Then
                MonthName = "September"
            ElseIf Month = "10" Then
                MonthName = "October"
            ElseIf Month = "11" Then
                MonthName = "November"
            ElseIf Month = "12" Then
                MonthName = "December"
            End If
            Dim Day As String = TextBoxDate.ToString("dddd")
            Dim DayNumber As String = TextBoxDate.ToString("dd")
            If DayNumber.ToLower = "01" Then
                DayNumber = "1"
            ElseIf DayNumber.ToLower = "02" Then
                DayNumber = "2"
            ElseIf DayNumber.ToLower = "03" Then
                DayNumber = "3"
            ElseIf DayNumber.ToLower = "04" Then
                DayNumber = "4"
            ElseIf DayNumber.ToLower = "05" Then
                DayNumber = "5"
            ElseIf DayNumber.ToLower = "06" Then
                DayNumber = "6"
            ElseIf DayNumber.ToLower = "07" Then
                DayNumber = "7"
            ElseIf DayNumber.ToLower = "08" Then
                DayNumber = "8"
            ElseIf DayNumber.ToLower = "09" Then
                DayNumber = "9"
            End If
            FullDate = Day & ", " & MonthName & " " & DayNumber & ", " & TextBoxDate.ToString("yyyy")
        Else
            Dim Month As String = TextBoxDate.ToString("MM")
            Dim MonthName As String = ""
            If Month = "01" Then
                MonthName = "enero"
            ElseIf Month = "02" Then
                MonthName = "febrero"
            ElseIf Month = "03" Then
                MonthName = "marzo"
            ElseIf Month = "04" Then
                MonthName = "abril"
            ElseIf Month = "05" Then
                MonthName = "mayo"
            ElseIf Month = "06" Then
                MonthName = "junio"
            ElseIf Month = "07" Then
                MonthName = "julio"
            ElseIf Month = "08" Then
                MonthName = "agosto"
            ElseIf Month = "09" Then
                MonthName = "septiembre"
            ElseIf Month = "10" Then
                MonthName = "octubre"
            ElseIf Month = "11" Then
                MonthName = "noviembre"
            ElseIf Month = "12" Then
                MonthName = "diciembre"
            End If
            Dim Day As String = TextBoxDate.ToString("dddd")
            Dim DayName As String = ""
            If Day.ToLower = "monday" Then
                DayName = "Lunes"
            ElseIf Day.ToLower = "tuesday" Then
                DayName = "Martes"
            ElseIf Day.ToLower = "wednesday" Then
                DayName = "Miércoles"
            ElseIf Day.ToLower = "thursday" Then
                DayName = "Jueves"
            ElseIf Day.ToLower = "friday" Then
                DayName = "Viernes"
            ElseIf Day.ToLower = "saturday" Then
                DayName = "Sábado"
            ElseIf Day.ToLower = "sunday" Then
                DayName = "Domingo"
            End If
            Dim DayNumber As String = TextBoxDate.ToString("dd")
            If DayNumber.ToLower = "01" Then
                DayNumber = "1"
            ElseIf DayNumber.ToLower = "02" Then
                DayNumber = "2"
            ElseIf DayNumber.ToLower = "03" Then
                DayNumber = "3"
            ElseIf DayNumber.ToLower = "04" Then
                DayNumber = "4"
            ElseIf DayNumber.ToLower = "05" Then
                DayNumber = "5"
            ElseIf DayNumber.ToLower = "06" Then
                DayNumber = "6"
            ElseIf DayNumber.ToLower = "07" Then
                DayNumber = "7"
            ElseIf DayNumber.ToLower = "08" Then
                DayNumber = "8"
            ElseIf DayNumber.ToLower = "09" Then
                DayNumber = "9"
            End If
            FullDate = DayName & ", " & DayNumber & " de " & MonthName & " de " & TextBoxDate.ToString("yyyy")
        End If
        Return FullDate
    End Function
    Private Sub PublishReport(FullDate As String)
        Dim AccountFile As StreamReader
        If RadioButton1.Checked Then
            AccountFile = New StreamReader("IntroduceYourselfAccount.txt")
        ElseIf RadioButton2.Checked Then
            AccountFile = New StreamReader("BienvenidaAccount.txt")
        End If
        Dim currentline As String = ""
        Dim Account As String = ""
        Dim Key As String = ""
        While AccountFile.EndOfStream = False
            currentline = AccountFile.ReadLine
            If currentline.Contains("account") Then
                Dim GetAccount As String() = currentline.Split("=")
                Account = GetAccount(1)
            ElseIf currentline.Contains("key") Then
                Dim GetKey As String() = currentline.Split("=")
                Key = GetKey(1)
            End If
        End While
        Try
            Dim request As System.Net.WebRequest = System.Net.WebRequest.Create("https://api.steem.place/postToSteem/")
            request.Method = "POST"
            Dim postData As String = ""
            If RadioButton1.Checked Then
                postData = "title=Bienvenida a los usuarios el día " + FullDate + "&body=" + HttpUtility.UrlEncode(My.Computer.FileSystem.ReadAllText("report-bienvenida-" & TextBox1.Text.Replace("/", "-") & ".txt")) + "&author=" & Account & "&permlink=reporte-" + TextBox1.Text.Replace("/", "-") + "&tags=castellano,spanish,report,reporte,stats,bienvenida,estadisticas,data&pk=" & Key
            Else

                postData = "title=IntroduceYourself posts on " + FullDate + "&body=" + HttpUtility.UrlEncode(My.Computer.FileSystem.ReadAllText("report-introduceyourself-" & TextBox1.Text.Replace("/", "-") & ".txt")) + "&author=" & Account & "&permlink=report-" + TextBox1.Text.Replace("/", "-") + "&tags=report,reporte,stats,new,daily,data&pk=" & Key
            End If
            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = byteArray.Length
            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()
            Dim response As Net.WebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()
            reader.Close()
            dataStream.Close()
            response.Close()
            If responseFromServer = "ok" & vbLf Then
                MessageBox.Show("Report Generated And posted")
            Else
                MessageBox.Show("An Error occured While posting the report")
            End If
        Catch ex As Exception
            MessageBox.Show("An Error has occurred.")
        End Try
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If ListBox2.SelectedIndex >= 0 Then
            Dim SQLQuery2 As String = ""
            If RadioButton1.Checked Then
                SQLQuery2 = "UPDATE bienvenida Set verified=1 WHERE link = '" & ListBox2.SelectedItem & "'"
            Else
                SQLQuery2 = "UPDATE introduceyourself SET verified=1 WHERE link = '" & ListBox2.SelectedItem & "'"
            End If
            Dim Connection2 As MySqlConnection = New MySqlConnection(MySQLString)
            Dim Command2 As New MySqlCommand(SQLQuery2, Connection2)
            Connection2.Open()
            Command2.ExecuteNonQuery()
            Connection2.Close()
            ListBox1.Items.Add(ListBox2.SelectedItem)
            ListBox2.Items.RemoveAt(ListBox2.SelectedIndex)
            If ListBox2.Items.Count > 0 Then
                ListBox2.SelectedIndex = 0
            End If
        End If
    End Sub

    Private Sub ListBox2_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListBox2.MouseDoubleClick
        If ListBox2.SelectedIndex >= 0 Then Process.Start("https://steemit.com/tag/" & ListBox2.SelectedItem)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If ListBox2.SelectedIndex >= 0 Then
            Dim SQLQuery2 As String = ""
            If RadioButton1.Checked Then
                SQLQuery2 = "UPDATE bienvenida SET verified=2 WHERE link = '" & ListBox2.SelectedItem & "'"
            Else
                SQLQuery2 = "UPDATE introduceyourself SET verified=2 WHERE link = '" & ListBox2.SelectedItem & "'"
            End If
            Dim Connection2 As MySqlConnection = New MySqlConnection(MySQLString)
            Dim Command2 As New MySqlCommand(SQLQuery2, Connection2)
            Connection2.Open()
            Command2.ExecuteNonQuery()
            Connection2.Close()
            ListBox3.Items.Add(ListBox2.SelectedItem)
            ListBox2.Items.RemoveAt(ListBox2.SelectedIndex)
            If ListBox2.Items.Count > 0 Then
                ListBox2.SelectedIndex = 0
            End If
        End If
    End Sub

    Private Sub ListBox3_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListBox3.MouseDoubleClick
        If ListBox3.SelectedIndex >= 0 Then Process.Start("https://steemit.com/tag/" & ListBox3.SelectedItem)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim AccountFile As StreamReader = New StreamReader("IntroduceYourselfAccount.txt")
        Dim currentline As String = ""
        Dim Account As String = ""
        Dim Key As String = ""
        While AccountFile.EndOfStream = False
            currentline = AccountFile.ReadLine
            If currentline.Contains("account") Then
                Dim GetAccount As String() = currentline.Split("=")
                Account = GetAccount(1)
            ElseIf currentline.Contains("key") Then
                Dim GetKey As String() = currentline.Split("=")
                Key = GetKey(1)
            End If
        End While
        If ListBox3.SelectedIndex >= 0 Then
            Dim Tags As String = ""
            Dim Counter As Integer = 0
            If CheckBox1.Checked Then
                If Counter < 1 Then
                    Tags = Tags + "#introduceyourself"
                Else
                    Tags = Tags + ", #introduceyourself"
                End If
                Counter = Counter + 1
            End If
            If CheckBox2.Checked Then
                If Counter < 1 Then
                    Tags = Tags + "#introducemyself"
                Else
                    Tags = Tags + ", #introducemyself"
                End If
                Counter = Counter + 1
            End If
            If CheckBox3.Checked Then
                If Counter < 1 Then
                    Tags = Tags + "#introduction"
                Else
                    Tags = Tags + ", #introduction"
                End If
                Counter = Counter + 1
            End If
            Dim Text As String = ""
            If CheckBox4.Checked = False Then
                Text = "Please do not use the "
                Text = Text & Tags
                If Counter > 1 Then
                    Text = Text & " tags "
                Else
                    Text = Text & " tag "
                End If
                Text = Text & "on posts that are not introductions."
            Else
                Text = "Por favor, no utilice "
                If Counter > 1 Then
                    Text = Text & "los tags "
                Else
                    Text = Text & "el tag "
                End If
                Text = Text & Tags
                Text = Text & " en posts que no son de introducción."
            End If
            Try
                Dim request As System.Net.WebRequest = System.Net.WebRequest.Create("https://api.steem.place/postReply/")
                request.Method = "POST"
                Dim postData As String = "i=" & ListBox3.SelectedItem & "&b=" & Text & "&a=" & Account & " &pk=" & Key
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
                request.ContentType = "application/x-www-form-urlencoded"
                request.ContentLength = byteArray.Length
                Dim dataStream As Stream = request.GetRequestStream()
                dataStream.Write(byteArray, 0, byteArray.Length)
                dataStream.Close()
                Dim response As Net.WebResponse = request.GetResponse()
                dataStream = response.GetResponseStream()
                Dim reader As New StreamReader(dataStream)
                Dim responseFromServer As String = reader.ReadToEnd()
                reader.Close()
                dataStream.Close()
                response.Close()
                If responseFromServer = "ok" Then
                    MessageBox.Show("Reply posted successfully")
                Else
                    MessageBox.Show("An error occured while replying.")
                End If
            Catch ex As Exception
                MessageBox.Show("An error has occurred.")
            End Try
        End If
    End Sub

    Private Function GetPostTitle(Link As String)
        Link = Link.Remove(0, 1)
        Try
            Dim myWebRequest As System.Net.WebRequest = System.Net.WebRequest.Create("https://api.steem.place/getPostTitle/?p=" & Link)
            Dim myWebResponse As System.Net.WebResponse = myWebRequest.GetResponse()
            Dim ReceiveStream As Stream = myWebResponse.GetResponseStream()
            Dim encode As Encoding = System.Text.Encoding.GetEncoding("utf-8")
            Dim readStream As New StreamReader(ReceiveStream, encode)
            Return readStream.ReadLine
        Catch ex As Exception
            GetPostTitle(Link)
        End Try
    End Function
    Private Async Sub SendMessage(fulldate As String, datestring As String)
        Dim Channel As DiscordChannel = Await DiscordClient.GetChannelAsync(369214082349268992)
        Await DiscordClient.SendMessageAsync(Channel, fulldate & ": https://steemit.com/tag/@bienvenida/reporte-" & datestring.Replace("/", "-"))
    End Sub
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim MySQLFile As StreamReader = New StreamReader("MySQLConfig.txt")
        Dim currentline As String = ""
        Dim MySQLServer As String = ""
        Dim MySQLUser As String = ""
        Dim MySQLPassword As String = ""
        Dim MySQLDatabase As String = ""
        While MySQLFile.EndOfStream = False
            currentline = MySQLFile.ReadLine
            If currentline.Contains("server") Then
                Dim GetServer As String() = currentline.Split("=")
                MySQLServer = GetServer(1)
            ElseIf currentline.Contains("username") Then
                Dim GetUsername As String() = currentline.Split("=")
                MySQLUser = GetUsername(1)
            ElseIf currentline.Contains("password") Then
                Dim GetPassword As String() = currentline.Split("=")
                MySQLPassword = GetPassword(1)
            ElseIf currentline.Contains("database") Then
                Dim GetDatabase As String() = currentline.Split("=")
                MySQLDatabase = GetDatabase(1)
            End If
        End While
        MySQLString = "server=" + MySQLServer + ";user=" + MySQLUser + ";database=" + MySQLDatabase + ";port=3306;password=" + MySQLPassword + ";"
        Dim dcfg As New DiscordConfig
        With dcfg
            .Token = My.Computer.FileSystem.ReadAllText("token.txt")
            .TokenType = TokenType.Bot
            .LogLevel = LogLevel.Debug
            .AutoReconnect = True
        End With
        Me.DiscordClient = New DiscordClient(dcfg)
        Me.DiscordClientLogger = Me.DiscordClient.DebugLogger
        Await Me.DiscordClient.ConnectAsync()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If ListBox1.SelectedIndex >= 0 Then
            Dim SQLQuery2 As String = ""
            If RadioButton1.Checked Then
                SQLQuery2 = "UPDATE bienvenida SET verified=0 WHERE link = '" & ListBox1.SelectedItem & "'"
            Else
                SQLQuery2 = "UPDATE introduceyourself SET verified=0 WHERE link = '" & ListBox1.SelectedItem & "'"
            End If
            Dim Connection2 As MySqlConnection = New MySqlConnection(MySQLString)
            Dim Command2 As New MySqlCommand(SQLQuery2, Connection2)
            Connection2.Open()
            Command2.ExecuteNonQuery()
            Connection2.Close()
            RefreshListBox()
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If ListBox3.SelectedIndex >= 0 Then
            Dim SQLQuery2 As String = ""
            If RadioButton1.Checked Then
                SQLQuery2 = "UPDATE bienvenida SET verified=0 WHERE link = '" & ListBox3.SelectedItem & "'"
            Else
                SQLQuery2 = "UPDATE introduceyourself SET verified=0 WHERE link = '" & ListBox3.SelectedItem & "'"
            End If
            Dim Connection2 As MySqlConnection = New MySqlConnection(MySQLString)
            Dim Command2 As New MySqlCommand(SQLQuery2, Connection2)
            Connection2.Open()
            Command2.ExecuteNonQuery()
            Connection2.Close()
            RefreshListBox()
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        PublishReport(GetFullDate(DateTime.ParseExact(TextBox1.Text, "MM/dd/yyyy", Nothing)))
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If ListBox2.Items.Count >= 30 Then
            For i As Integer = 0 To 29
                Process.Start("https://steemit.com/tag/" & ListBox2.Items.Item(i))
                Threading.Thread.Sleep(300)
            Next
        Else
            For i As Integer = 0 To ListBox2.Items.Count - 1
                Process.Start("https://steemit.com/tag/" & ListBox2.Items.Item(i))
                Threading.Thread.Sleep(300)
            Next
        End If
    End Sub
End Class
