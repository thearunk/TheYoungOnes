# TheYoungOnes

Console app to get the 5 youngest users
Program.cs
  - This code makes calls to the list service to get user ids and creates async tasks to 
    query the details service for every user
  - The async details task updates the youngest users list for users with a valid phone
  - Phone number validity is checked by a regular expression for area code within parentheses (optional) 
    and for white space and - separators
    
User.cs
  - user details
  
PartialUserList.cs
  - list of user ids
    
