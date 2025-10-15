# <p align="center"> <img id="top" src="wwwroot/Images/logo_transparent.png" alt="CivicLink Logo" width="100"/> </p>

<h1 align="center">PROG7312 POE - CivicLink</h1>
<br>
<h2 align="center">About this Project</h2>
<p align="center">CivicLink is a comprehensive municipal services application designed to bridge the gap between citizens and their local government. Built with the goal of improving community engagement and service delivery, CivicLink allows residents to report issues, stay informed about local events and announcements, and track service requests. The application features an elegant Apple-inspired design with gamification elements to encourage active civic participation. Through innovative data structures and a user-friendly interface, citizens can easily contribute to making their communities better while earning rewards for their engagement.</p>
<br><br>
<p align="center">
  <strong>Jordan Muller | ST10150702</strong>
 <br>
 Youtube Link to Demonstrative Video for Part 2: <br>
 https://youtu.be/5Nd3AzEvqM0 <br><br>
</p>
<br>

## Built With

<div>
  <a href="https://docs.microsoft.com/en-us/aspnet/core/" target="_blank">
    <img src="https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white&width=200" alt="ASP.NET Core Badge">
  </a>
  <br>
  <a href="https://docs.microsoft.com/en-us/dotnet/csharp/" target="_blank">
    <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white&width=200" alt="C# Badge">
  </a>
  <br>
  <a href="https://getbootstrap.com/" target="_blank">
    <img src="https://img.shields.io/badge/Bootstrap-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white&width=200" alt="Bootstrap Badge">
  </a>
  <br>
  <a href="https://github.com/" target="_blank">
    <img src="https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white&width=200" alt="GitHub Badge">
  </a>
  <br>
  <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript" target="_blank">
    <img src="https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black&width=200" alt="JavaScript Badge">
  </a>
 <br><br>
 <a href="#top">(Back to Top)</a>
</div>

<br>

## Getting Started

<div>
This project is created using ASP.NET Core 8.0 with MVC architecture, following clean coding principles and advanced data structures.

### Prerequisites

- **Visual Studio 2022** or **Visual Studio Code**
- **.NET 8.0 SDK** or later
- **Git** for version control

Ensure you have the latest version of Visual Studio by following these steps:
<br> Download Visual Studio <a href="https://visualstudio.microsoft.com/downloads/" target="_blank">here</a> and ensure you have the ASP.NET and web development workload installed.

### Cloning Repository

To clone this repository:

```bash
git clone https://github.com/VCCT-PROG7312-2025-G2/ST10150702_Jordan_Muller_CivicLink.git
cd CivicLink
```

### Running the Project

1. **Open the solution** in Visual Studio 2022
2. **Restore NuGet packages** (usually done automatically)
3. **Build the solution** (Ctrl+Shift+B)
4. **Run the application** (F5 or Ctrl+F5)

### Project Structure

```
CivicLink/
├── Controllers/           # MVC Controllers
├── Models/               # Data models and ViewModels
├── Views/                # Razor views
├── Services/             # Business logic services
├── DataStructures/       # Custom data structures
├── wwwroot/             # Static files (CSS, JS, images)
│   └── Images/          # Logo and other images
└── Program.cs           # Application entry point
```

</div>
<a href="#top">(Back to Top)</a>

<br><br>

## Usage

This application is designed for municipal residents who want to actively participate in improving their communities. The system provides a streamlined way to report issues, discover local events, track progress, and engage with local government services.

<div align="center">
This Application has the following Features:<br>
  <img src="https://img.shields.io/badge/Issue%20Reporting-blue?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Local%20Events-cyan?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Event%20Recommendations-teal?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Gamification%20System-green?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Progress%20Tracking-orange?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Apple%20Design-purple?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Custom%20Data%20Structures-red?style=for-the-badge" />
  <img src="https://img.shields.io/badge/MVC%20Architecture-gray?style=for-the-badge" />
</div>
<br>

Users can report various types of municipal issues, discover upcoming community events, earn points and badges for participation, and track the status of their submissions in real-time.

### Screenshots

<!-- Home Page -->
|  <img src="wwwroot/Images/Home_Page.png" alt="home image"> | **Home Page**<br>The **Home Page** welcomes users with an elegant Apple-inspired design featuring the CivicLink logo, user engagement statistics, and easy access to municipal services. Users can see their current level, points earned, and badges unlocked. The page displays community statistics and provides quick actions to report issues, view local events, or check all submissions. |
|---|---|

<!-- Report Issue Page -->
| **Report Issue Page**<br>The **Report Issue Page** allows citizens to submit detailed reports about municipal problems. Users can select categories (Water & Sanitation, Roads & Transport, Public Safety, etc.), set priority levels, add descriptions, provide contact information, and attach photos or documents. The gamification system rewards users with points for each submission. | <img src="wwwroot/Images/Report_Issue.png" alt="Report image"> |
|---|---|

<!-- Issues List Page -->
|  <img src="wwwroot/Images/All_Issues.png" alt="All Issues image"> | **Issues List Page**<br>The **Issues List Page** displays all reported issues with filtering and search capabilities. Users can filter by category, status, and search by keywords. Each issue card shows priority level, status, location, and creation date. The page includes real-time statistics and visual indicators for issue priority and status. |
|---|---|

<!-- Issue Details Page -->
| **Issue Details Page**<br>The **Issue Details Page** provides comprehensive information about a specific issue, including a status timeline, contact information, location details, and any attached files. Users can share issues, report updates, and follow issues for notifications. The page features a clean timeline showing the progress from submission to resolution. |  <img src="wwwroot/Images/Inspect_Issue.png" alt="Inspect image"> |
|---|---|

<!-- Local Events Page -->
|  <img src="wwwroot/Images/Local_Events.png" alt="Local Events image"> | **Local Events & Announcements**<br>The **Local Events Page** displays all upcoming community events, town halls, and municipal announcements. Users can search and filter events by category, date, and location. The page features personalized event recommendations based on user search patterns, with each event card displaying key information including date, time, location, and cost. Advanced data structures power the efficient search and recommendation algorithms. |
|---|---|

<!-- Event Details Page -->
| **Event Details Page**<br>The **Event Details Page** provides comprehensive information about a specific event including full description, date and time, location details, and contact information. Users can view all event details in an elegant layout with clear visual hierarchy. The page tracks recently viewed events and helps users discover related community happenings. |  <img src="wwwroot/Images/View_Event.png" alt="View Event image"> |
|---|---|

<br>
<a href="#top">(Back to Top)</a>

<br><br>

## Technical Architecture

### Custom Data Structures

**Part 1 - Issue Reporting:**
- **LinkedList Implementation**: Stores issues without using primitive arrays
- **Priority Queue**: Manages issues by priority level using heap structure  
- **Activity Stack**: Tracks recent user activities and system events

**Part 2 - Local Events & Announcements:**
- **SortedDictionary**: Stores events with efficient lookup and sorting by date
- **Search Pattern Tracker**: Dictionary-based tracking of user search behavior for recommendations
- **Recently Viewed Stack**: Stack implementation for tracking recently viewed events
- **Category Manager**: HashSet for managing unique event categories
- **Queue for Recommendations**: Queue structure for personalized event suggestions

**Architecture Principles:**
- **No Business Logic in Controllers**: Clean MVC separation with service layer
- **Service-Oriented Design**: IssueService, EventService, and GamificationService
- **Dependency Injection**: Proper service registration and lifecycle management

### Key Features
- **Apple-inspired UI**: Modern, elegant design with smooth animations
- **Gamification System**: Points, levels, and badges to encourage participation
- **Event Recommendation Engine**: Personalized suggestions based on search patterns
- **Advanced Search & Filtering**: Multi-criteria filtering for both issues and events
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **File Upload Support**: Attach photos and documents to issue reports
- **Status Tracking**: Real-time updates on issue resolution progress
- **Event Discovery**: Browse, search, and filter upcoming community events

<br><br>

## Roadmap

- ✅ **Part 1 - Issue Reporting** (Complete)
  - ✅ Complete issue submission system
  - ✅ Category and priority management
  - ✅ File attachment support
  - ✅ Gamification with points and badges
  - ✅ Apple-inspired UI design
  - ✅ Custom data structures (LinkedList, Priority Queue, Stack)

- ✅ **Part 2 - Local Events & Announcements** (Complete)
  - ✅ Local Events browsing and discovery
  - ✅ Upcoming Events display with detailed information
  - ✅ Advanced Search and Filter functionality
  - ✅ Event Recommendations based on user behavior
  - ✅ Advanced Data Structures (SortedDictionary, HashSet, Queue, Stack)
  - ✅ Search Pattern Tracking algorithm

- ⬜ **Part 3 - Service Request Status** (Future)
  - ⬜ Service Request tracking dashboard
  - ⬜ Real-time status updates
  - ⬜ Database integration for persistent storage
  - ⬜ User authentication and profiles
  - ⬜ Complete end-to-end municipal services platform


<br>
<a href="#top">(Back to Top)</a>

<br><br>

## Code Architecture

### Models
**Issue Reporting:**
- **Issue**: Core issue entity with validation
- **UserEngagement**: Gamification tracking
- **Badge**: Achievement system
- **ReportIssueViewModel**: View model for issue reporting
- **Enums**: IssueCategory, IssuePriority, IssueStatus, BadgeType

**Events & Announcements:**
- **Event**: Core event entity with detailed information
- **EventsViewModel**: View model for events listing with filters
- **Enums**: EventCategory (CommunityMeeting, TownHall, PublicHearing, Recreation, Education, Health, Safety, Environment, Arts, Sports, Festival, Workshop)

### Services  
- **IssueService**: Issue management using custom linked list and priority queue
- **EventService**: Event management with search, filtering, and recommendation algorithms
- **GamificationService**: User engagement and badge system
- **Dependency Injection**: Clean service registration and lifecycle management

### Data Structures

**Part 1 - Issue Data Structures:**
- **IssueLinkedList**: Custom linked list implementation
- **IssuePriorityQueue**: Heap-based priority queue
- **ActivityStack**: Stack for activity logging

**Part 2 - Event Data Structures:**
- **EventSortedDictionary**: SortedDictionary for efficient event storage and retrieval
- **SearchPatternTracker**: Dictionary-based search pattern analysis
- **RecentlyViewedStack**: Stack for tracking recently viewed events
- **CategoryManager**: HashSet for unique category management

<br><br>

## Contributors

**Individual Project by:**

<table align="center">
  <tr>
    <td align="center">
      <div style="width: 120px; height: 120px; border-radius: 50%; overflow: hidden;">
        <img src="wwwroot/Images/JordanMullerGitHub.png" alt="github image" width="120" height="120" style="object-fit: cover;">
      </div>
      <strong>Jordan Muller ST10150702</strong>
    </td>
  </tr>
</table>

<br>
<a href="#top">(Back to Top)</a>

<br><br>


## Contact

**Jordan Muller** - ST10150702@vcconnect.edu.za
<br>
**Project Link**: https://github.com/VCCT-PROG7312-2025-G2/CivicLink.git

<br>
<a href="#top">(Back to Top)</a>

<br><br>

## Acknowledgments

With extensive research into municipal services, user experience design, and modern web development practices, the following acknowledgments need to be made:

### References

Microsoft. (2024). ASP.NET Core documentation. Retrieved from Microsoft Docs: https://docs.microsoft.com/en-us/aspnet/core/

Microsoft. (2024). C# programming guide. Retrieved from Microsoft Docs: https://docs.microsoft.com/en-us/dotnet/csharp/

Microsoft. (2024). LINQ and async programming patterns. Retrieved from Microsoft Docs: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/

Apple Inc. (2024). Human Interface Guidelines. Retrieved from Apple Developer: https://developer.apple.com/design/human-interface-guidelines/

Bootstrap Team. (2024). Bootstrap Documentation. Retrieved from Bootstrap: https://getbootstrap.com/docs/

Stack Overflow. (2024). ASP.NET Core MVC best practices. Retrieved from Stack Overflow community discussions.

Mozilla. (2024). Web APIs and modern JavaScript. Retrieved from MDN Web Docs: https://developer.mozilla.org/

Government of South Africa. (2024). Municipal services and citizen engagement. Retrieved from various municipal websites and documentation.

### AI Usage

In the development of this project, AI tools were utilized as supplementary resources to enhance understanding of complex programming concepts and explore modern design patterns. These tools provided guidance but all implementation work was performed by the developer.

**OpenAI**: OpenAI, 2024. ChatGPT. Available at: https://openai.com [Accessed throughout development].

**Claude AI**: Anthropic, 2024. Claude. Available at: https://claude.ai [Accessed for architectural guidance and problem-solving].

AI assistance was primarily used for:
- Understanding complex data structure implementations (SortedDictionary, search algorithms)
- Exploring Apple design principles and CSS techniques  
- Reviewing MVC best practices and clean architecture
- Debugging and optimization suggestions
- Algorithm design for event recommendation system
- Learning advanced LINQ queries and async/await patterns

All code was written, understood, and tested by the developer. AI tools served as educational resources similar to documentation and Stack Overflow.

<br>


<a href="#top">(Back to Top)</a>

