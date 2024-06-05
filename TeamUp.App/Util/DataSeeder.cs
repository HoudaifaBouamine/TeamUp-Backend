using Authentication.UserManager;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;

public static class DataSeeder
{

    // public static async Task SeedFollowersData(AppDbContext db)
    // {
    //     var projectFaker = new Faker<ProjectPostFaker>("en_US");
    //
    // }
    public static async Task SeedCaterogyData(AppDbContext db)
    {  
        string[] categories = ["Mobile","Design","Web", "Cyber security","Ai", "Game", "Data Science"];
        // string[] categories = ["Web Dev","Design","Mobile Dev","Cyber security"];
        await db.Categories.AddRangeAsync(categories.Select(c=>new Category
        {
            Name = c
        }));
        await db.SaveChangesAsync();
    }
    
    class ProjectPostFaker
    {
        public User Creator { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Sinarios { get; set; }
        public string learningGoals { get; set; }
        public string teamAndRols { get; set; }

        public List<Category> categories { get; set; }
        public DateTime PostingTime { get; set; }
    }
    public static async Task SeedProjectPostData(AppDbContext db)
    {
        var users = await db.Users.ToListAsync();
        var skills = await db.Skills.ToListAsync();
        var categories = await db.Categories.ToListAsync();
        var projectFaker = new Faker<ProjectPostFaker>("en_US");

        projectFaker
            .RuleFor(u => u.Creator,
                (f, p) => users
                    .Where(u=>u.IsMentor)
                    .Where((u, index) => new Random().Next(5) == 0 || index == 0)
                    .Last())
            .RuleFor(u => u.Title, (f, p) => f.Commerce.ProductName())
            .RuleFor(u => u.Sinarios, (f, p) => f.Lorem.Paragraph())
            .RuleFor(u => u.learningGoals, (f, p) => f.Lorem.Paragraph())
            .RuleFor(u => u.teamAndRols, (f, p) => f.Lorem.Paragraph())
            .RuleFor(u => u.Summary, (f, p) => f.Lorem.Sentence(20, 5))
            .RuleFor(u => u.PostingTime, f => f.Date.Between(new DateTime(2023, 1, 6), DateTime.UtcNow))
            .RuleFor(u => u.categories, f=>
            {
                var cs = categories.ToList()
                    .Where(c => f.Random.Bool())
                    .ToList();
                
                if(cs.Count == 0)
                    cs.Add(categories[new Random().Next(0,categories.Count-1)]);

                return cs;
            });
        var projectsToCreat = projectFaker.Generate(10);
        
        projectFaker.RuleFor(u => u.Creator,
            users.FirstOrDefault(u => u.Email == "string@gmail.com"));
        
        projectsToCreat.AddRange(projectFaker.Generate(3));

        Log.Debug("Project Posts Count : " + projectsToCreat.Count);
        
        string[] expectedDuration = ["1 Week", "2-3 Weeks", "1 Month", "2-3 Months", "+3 Months"];

        var projects = projectsToCreat
            .Select(p => new ProjectPost
            (
                p.Creator,
                p.Title,
                p.Summary,
                expectedDuration[new Random().Next(expectedDuration.Length)],
                new Random().Next(20) + 5,
                p.Sinarios, 
                p.learningGoals,
                p.teamAndRols,
                skills.ToList().Where(s=> new Random().Next((int)Math.Ceiling(skills.Count * 70.0 / 1000.0)) == 0).ToList(), 
                p.categories.ToList()
            )
            {
                PostingTime = p.PostingTime
            });

      await db.ProjectPosts.AddRangeAsync(projects);
      await db.SaveChangesAsync();
    }
    class UserCreateFaker
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PictureUrl { get; set; }
        public string Handler { get; set; }
        public bool IsMentor { get; set; }
        public bool Categories { get; set; }
    }

    // record UserCreateFake(string FirstName, string LastName, string Email, string PicureUrl);
    public static async Task SeedUsersData(AppDbContext db, UserManager<User> userManager = null)
    {
        var userFaker = new Faker<UserCreateFaker>();

        userFaker
            .RuleFor(u => u.FirstName, (f, p) => f.Name.FirstName())
            .RuleFor(u => u.LastName, (f, p) => f.Name.LastName())
            .RuleFor(u => u.Email, (f, p) => f.Internet.Email(p.FirstName, p.LastName))
            .RuleFor(u => u.PictureUrl, f =>
            {
                var parts = f.Person.Avatar.Split("cloudflare-ipfs.com");
                return $"{parts[0]}ipfs.io{parts[1]}";
            })
            .RuleFor(u=>u.Handler, f=> f.Name.JobTitle() + " at " + f.Company.CompanyName())
            .RuleFor(u=>u.IsMentor , f=>f.Random.Bool())
            .RuleFor(u=>u.Categories , f=>f.Random.Bool());

        // https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/602.jpg
        var usersToCreat = userFaker.Generate(20);

        var users = usersToCreat.Select((u) =>
        {
            var mentor = new User(u.FirstName, u.LastName, u.Email, u.PictureUrl)
            {
                Handler = u.Handler,
                Categories = [],
            };

            if(u.IsMentor) mentor.SetAsMentor();
            
            return mentor;
        }).ToList();

        await db.Users.AddRangeAsync(users);
        // var user = new User
        // {
        //     DisplayName = "string",
        //     Handler = "string",
        //     Skills = db.Skills.ToList().Where(s => new Random().Next(3) == 0).ToList(),
        //     Categories = db.Categories.ToList().Where(s => new Random().Next(3) == 0).ToList()
        // };
        // await userManager.CreateAsync(user,"string");
        await db.SaveChangesAsync();
    }
    
    
    public static async Task SeedSkillsData(AppDbContext db)
    {
        List<string> skills = new List<string>
{
    "AI",
    "Web",
    "Programming",
    "Mobile",
    ".NET",
    "360-degree video",
    "3D Animation",
    "3D Design",
    "3D Model Maker",
    "3D Modelling",
    "3D Printing",
    "3D Rendering",
    "3ds Max",
    "4D",
    "Academic Writing",
    "Accounting",
    "ActionScript",
    "Active Directory",
    "Ad Planning / Buying",
    "Adobe Air",
    "Adobe Captivate",
    "Adobe Dreamweaver",
    "Adobe Fireworks",
    "Adobe Flash",
    "Adobe InDesign",
    // "Adobe Lightroom",
    // "Adobe LiveCycle Designer",
    // "Adobe Premiere Pro",
    // "Advertisement Design",
    // "Advertising",
    // "Aeronautical Engineering",
    // "Aerospace Engineering",
    // "Affiliate Marketing",
    // "Afrikaans",
    // "After Effects",
    // "Agile Development",
    // "Agronomy",
    // "Air Conditioning",
    // "Airbnb",
    // "AJAX",
    // "Albanian",
    // "Algorithm",
    // "Alibaba",
    // "Amazon Fire",
    // "Amazon Kindle",
    // "Amazon Web Services",
    // "AMQP",
    // "Analytics",
    // "Android",
    // "Android Honeycomb",
    // "Android Wear SDK",
    // "Angular.js",
    // "Animation",
    // "Antenna Services",
    // "Anything Goes",
    // "Apache",
    // "Apache Ant",
    // "Apache Solr",
    // "App Designer",
    // "App Developer",
    // "Appcelerator Titanium",
    // "Apple Compressor",
    // "Apple iBooks Author",
    // "Apple Logic Pro",
    // "Apple Motion",
    // "Apple Safari",
    // "Apple Watch",
    // "Applescript",
    // "Appliance Installation",
    // "Appliance Repair",
    // "Arabic",
    // "Arduino",
    // "Argus Monitoring Software",
    // "Article Rewriting",
    // "Article Submission",
    // "Articles",
    // "Artificial Intelligence",
    // "Arts / Crafts",
    // "AS400 / iSeries",
    // "Asbestos Removal",
    // "ASP",
    // "ASP.NET",
    // "Asphalt",
    // "Assembly",
    // "Asterisk PBX",
    // "Astrophysics",
    // "Attic Access Ladders",
    // "Attorney",
    // "Audio Production",
    // "Audio Services",
    // "Audit",
    // "Augmented Reality",
    // "AutoCAD",
    // "Autodesk Inventor",
    // "Autodesk Revit",
    // "AutoHotkey",
    // "Automotive",
    // "Autotask",
    // "Awnings",
    // "Axure",
    // "Azure",
    // "backbone.js",
    // "Balsamiq",
    // "Balustrading",
    // "Bamboo Flooring",
    // "Banner Design",
    // "Basque",
    // "Bathroom",
    // "Bengali",
    // "Big Data",
    // "BigCommerce",
    // "Binary Analysis",
    // "Biology",
    // "Biotechnology",
    // "Bitcoin",
    // "Biztalk",
    // "Blackberry",
    // "Blog",
    // "Blog Design",
    // "Blog Install",
    // "Bluetooth Low Energy (BLE)",
    // "BMC Remedy",
    // "Book Artist",
    // "Book Writing",
    // "Bookkeeping",
    // "Boonex Dolphin",
    // "Bootstrap",
    // "Bosnian",
    // "Bower",
    // "BPO",
    // "Brackets",
    // "Brain Storming",
    // "Branding",
    // "Bricklaying",
    // "Broadcast Engineering",
    // "Brochure Design",
    // "BSD",
    // "Building",
    // "Building Architecture",
    // "Building Certifiers",
    // "Building Consultants",
    // "Building Designer",
    // "Building Surveyors",
    // "Bulgarian",
    // "Bulk Marketing",
    // "Business Analysis",
    // "Business Cards",
    // "Business Catalyst",
    // "Business Coaching",
    // "Business Intelligence",
    // "Business Plans",
    // "Business Writing",
    // "Buyer Sourcing",
    // "C Programming",
    // "C# Programming",
    // "C++ Programming",
    // "CAD/CAM",
    // "CakePHP",
    // "Call Center",
    // "Call Control XML",
    // "Capture NX2",
    // "Caricature / Cartoons",
    // "Carpentry",
    // "Carpet Repair / Laying",
    // "Carports",
    // "Cartography / Maps",
    // "Carwashing",
    // "CasperJS",
    // "Cassandra",
    // "Catalan",
    // "Catch Phrases",
    // "CATIA",
    // "Ceilings",
    // "Cement Bonding Agents",
    // "CGI",
    // "Chef Configuration Management",
    // "Chemical Engineering",
    // "Chordiant",
    // "Christmas",
    // "Chrome OS",
    // "Cinema 4D",
    // "Circuit Design",
    // "Cisco",
    // "Civil Engineering",
    // "Classifieds Posting",
    // "Clean Technology",
    // "Cleaning Carpet",
    // "Cleaning Domestic",
    // "Cleaning Upholstery",
    // "Climate Sciences",
    // "CLIPS",
    // "Clothesline",
    // "Cloud Computing",
    // "CMS",
    // "Coating Materials",
    // "COBOL",
    // "Cocoa",
    // "Codeigniter",
    // "Coding",
    // "Cold Fusion",
    // "Columns",
    // "Commercial Cleaning",
    // "Commercials",
    // "Communications",
    // "Compliance",
    // "Computer Graphics",
    // "Computer Help",
    // "Computer Security",
    // "Concept Art",
    // "Concept Design",
    // "Concreting",
    // "Construction Monitoring",
    // "Content Writing",
    // "Contracts",
    // "Conversion Rate Optimisation",
    // "Cooking / Recipes",
    // "Cooking / Baking",
    // "Copy Typing",
    // "Copywriting",
    // "Corporate Identity",
    // "Courses",
    // "Covers / Packaging",
    // "CRE Loaded",
    // "Creative Design",
    // "Creative Writing",
    // "CRM",
    // "Croatian",
    // "Cryptography",
    // "Crystal Reports",
    // "CS-Cart",
    // "CSS",
    // "CubeCart",
    // "CUDA",
    // "Customer Service",
    // "Customer Support",
    // "Czech",
    // "Damp Proofing",
    // "Danish",
    // "Dari",
    // "Dart",
    // "Data Entry",
    // "Data Mining",
    // "Data Processing",
    // "Data Science",
    // "Data Warehousing",
    // "Database Administration",
    // "Database Development",
    // "Database Programming",
    // "DataLife Engine",
    // "Dating",
    // "DDS",
    // "Debian",
    // "Debugging",
    // "Decking",
    // "Decoration",
    // "Delivery",
    // "Delphi",
    // "Demolition",
    // "Design",
    // "Desktop Support",
    // "Digital Design",
    // "Disposals",
    // "Django",
    // "DNS",
    // "DOS",
    // "DotNetNuke",
    // "Drafting",
    // "Drains",
    // "Drones",
    // "Drupal",
    // "Dthreejs",
    // "Dutch",
    // "Dynamics",
    // "eBay",
    // "eBooks",
    // "eCommerce",
    // "Editing",
    // "Education / Tutoring",
    // "edX",
    // "Elasticsearch",
    // "eLearning",
    // "eLearning Designer",
    // "Electrical Engineering",
    // "Electricians",
    // "Electronic Forms",
    // "Electronics",
    // "Email Developer",
    // "Email Handling",
    // "Email Marketing",
    // "Embedded Software",
    // "Ember.js",
    // "Employment Law",
    // "Energy",
    // "Engineering",
    // "Engineering Drawing",
    // "English (UK)",
    // "English (US)",
    // "English Grammar",
    // "English Spelling",
    // "Entrepreneurship",
    // "ePub",
    // "Equipment Hire",
    // "Erlang",
    // "ERP",
    // "Estonian",
    // "Etsy",
    // "Event Planning",
    // "Event Staffing",
    // "Excavation",
    // "Excel",
    // "Express JS",
    // "Expression Engine",
    // "Extensions / Additions",
    // "Face Recognition",
    // "Facebook Marketing",
    // "Fashion Design",
    // "Fashion Modeling",
    // "Fencing",
    // "Feng Shui",
    // "Fiction",
    // "FileMaker",
    // "Filipino",
    // "Filmmaking",
    // "Final Cut Pro",
    // "Finale / Sibelius",
    // "Finance",
    // "Financial Analysis",
    // "Financial Markets",
    // "Financial Planning",
    // "Financial Research",
    // "Finite Element Analysis",
    // "Finnish",
    // "Firefox",
    // "Flash 3D",
    // "Flashmob",
    // "Flex",
    // "Floor Coatings",
    // "Flooring",
    // "Flow Charts",
    // "Flyer Design",
    // "Flyscreens",
    // "Food Takeaway",
    // "Format / Layout",
    // "Fortran",
    // "Forum Posting",
    // "Forum Software",
    // "FPGA",
    // "Frames / Trusses",
    // "Freelance",
    // "FreelancerAPI",
    // "FreeSwitch",
    // "French",
    // "French (Canadian)",
    // "Fundraising",
    // "Furniture Assembly",
    // "Furniture Design",
    // "Game Consoles",
    // "Game Design",
    // "Game Development",
    // "GameSalad",
    // "Gamification",
    // "GarageBand",
    // "Gardening",
    // "Gas Fitting",
    // "Genealogy",
    // "General Labor",
    // "General Office",
    // "Genetic Engineering",
    // "Geolocation",
    // "Geology",
    // "Geospatial",
    // "Geotechnical Engineering",
    // "German",
    // "Ghostwriting",
    // "GIMP",
    // "Git",
    // "Glass / Mirror / Glazing",
    // "Golang",
    // "Google Adsense",
    // "Google Adwords",
    // "Google Analytics",
    // "Google App Engine",
    // "Google Cardboard",
    // "Google Chrome",
    // "Google Cloud Storage",
    // "Google Earth",
    // "Google Maps API",
    // "Google Plus",
    // "Google SketchUp",
    // "Google Web Toolkit",
    // "Google Webmaster Tools",
    // "Google Website Optimizer",
    // "GoPro",
    // "GPGPU",
    // "GPS",
    // "Grails",
    // "Grant Writing",
    // "Graphic Design",
    // "Grease Monkey",
    // "Greek",
    // "Growth Hacking",
    // "Grunt",
    // "Guttering",
    // "Hadoop",
    // "Hair Styles",
    // "Handyman",
    // "Haskell",
    // "HBase",
    // "Health",
    // "Heating Systems",
    // "Hebrew",
    // "Helpdesk",
    // "Heroku",
    // "Hindi",
    // "Hire me",
    // "History",
    // "Hive",
    // "Home Automation",
    // "Home Design",
    // "Home Organization",
    // "HomeKit",
    // "Hot Water",
    // "House Cleaning",
    // "Housework",
    // "HP Openview",
    // "HTML",
    // "HTML5",
    // "Human Resources",
    // "Human Sciences",
    // "Hungarian",
    // "iBeacon",
    // "IBM BPM",
    // "IBM Tivoli",
    // "IBM Websphere Transformation Tool",
    // "Icon Design",
    // "IIS",
    // "IKEA Installation",
    // "Illustration",
    // "Illustrator",
    // "Imaging",
    // "iMovie",
    // "Indonesian",
    // "Industrial Design",
    // "Industrial Engineering",
    // "Infographics",
    // "Inspections",
    // "Instagram",
    // "Installation",
    // "Instrumentation",
    // "Insurance",
    // "Interior Design",
    // "Interiors",
    // "Internet Marketing",
    // "Internet Research",
    // "Internet Security",
    // "Interpreter",
    // "Interspire",
    // "Intuit QuickBooks",
    // "Inventory Management",
    // "Investment Research",
    // "Invitation Design",
    // "Ionic Framework",
    // "iPad",
    // "iPhone",
    // "ISO9001",
    // "Italian",
    // "ITIL",
    // "J2EE",
    // "J2ME",
    // "Jabber",
    // "Japanese",
    // "Java",
    // "JavaFX",
    // "Javascript",
    // "JD Edwards CNC",
    // "JDF",
    // "Jewellery",
    // "Joomla",
    // "Journalist",
    // "jQuery / Prototype",
    // "JSON",
    // "JSP",
    // "Kannada",
    // "Kinect",
    // "Kitchen",
    // "Knockout.js",
    // "Korean",
    // "Label Design",
    // "LabVIEW",
    // "Landing Pages",
    // "Landscape Design",
    // "Landscaping",
    // "Landscaping / Gardening",
    // "Laravel",
    // "LaTeX",
    // "Latvian",
    // "Laundry and Ironing",
    // "Lawn Mowing",
    // "Leads",
    // "Leap Motion SDK",
    // "Legal",
    // "Legal Research",
    // "Legal Writing",
    // "LESS/Sass/SCSS",
    // "Life Coaching",
    // "Lighting",
    // "Linear Programming",
    // "Link Building",
    // "Linkedin",
    // "Linnworks Order Management",
    // "LINQ",
    // "Linux",
    // "Lisp",
    // "Lithuanian",
    // "LiveCode",
    // "Locksmith",
    // "Logistics / Shipping",
    // "Logo Design",
    // "Lotus Notes",
    // "Lua",
    // "Mac OS",
    // "Macedonian",
    // "Machine Learning",
    // "Magento",
    // "Magic Leap",
    // "Mailchimp",
    // "Mailwizz",
    // "Make Up",
    // "Makerbot",
    // "Malay",
    // "Malayalam",
    // "Maltese",
    // "Management",
    // "Manufacturing",
    // "Manufacturing Design",
    // "Map Reduce",
    // "MariaDB",
    // "Market Research",
    // "Marketing",
    // "Marketplace Service",
    // "Materials Engineering",
    // "Mathematics",
    // "Matlab and Mathematica",
    // "Maya",
    // "Mechanical Engineering",
    // "Mechatronics",
    // "Medical",
    // "Medical Writing",
    // "Metatrader",
    // "MeteorJS",
    // "Metro",
    // "Microbiology",
    // "Microcontroller",
    // "Microsoft",
    // "Microsoft Access",
    // "Microsoft Exchange",
    // "Microsoft Expression",
    // "Microsoft Hololens",
    // "Microsoft Office",
    // "Microsoft Outlook",
    // "Microsoft SQL Server",
    // "Microsoft Visio",
    // "Microstation",
    // "Millwork",
    // "Mining Engineering",
    // "Minitlab",
    // "MLM",
    // "MMORPG",
    // "Mobile App Testing",
    // "Mobile Phone",
    // "MODx",
    // "MonetDB",
    // "Moodle",
    // "Mortgage Brokering",
    // "Motion Graphics",
    // "Moving",
    // "MQTT",
    // "Mural Painting",
    // "Music",
    // "MVC",
    // "MYOB",
    // "MySpace",
    // "MySQL",
    // "Nanotechnology",
    // "Natural Language",
    // "Network Administration",
    // "Newsletters",
    // "Nginx",
    // "Ning",
    // "Nintex Forms",
    // "Nintex Workflow",
    // "node.js",
    // "Nokia",
    // "Norwegian",
    // "NoSQL Couch / Mongo",
    // "Nutrition",
    // "OAuth",
    // "Objective C",
    // "OCR",
    // "Oculus Mobile SDK",
    // "Odoo",
    // "Online Writing",
    // "Open Cart",
    // "Open Journal Systems",
    // "OpenBravo",
    // "OpenCL",
    // "OpenGL",
    // "OpenSceneGraph",
    // "OpenSSL",
    // "OpenStack",
    // "OpenVMS",
    // "Oracle",
    // "Order Processing",
    // "Organizational Change Management",
    // "OSCommerce",
    // "Package Design",
    // "Packing / Shipping",
    // "Painting",
    // "Palm",
    // "Papiamento",
    // "Parallax Scrolling",
    // "Parallel Processing",
    // "Parallels Automation",
    // "Parallels Desktop",
    // "Patents",
    // "Pattern Making",
    // "Pattern Matching",
    // "Pavement",
    // "PayPal API",
    // "Payroll",
    // "Paytrace",
    // "PCB Layout",
    // "PDF",
    // "PEGA PRPC",
    // "PencilBlue CMS",
    // "Pentaho",
    // "PeopleSoft",
    // "Periscope",
    // "Perl",
    // "Personal Development",
    // "Pest Control",
    // "Pet Sitting",
    // "Petroleum Engineering",
    // "Phone Support",
    // "PhoneGap",
    // "Photo Editing",
    // "Photography",
    // "Photoshop",
    // "Photoshop Coding",
    // "Photoshop Design",
    // "PHP",
    // "Physics",
    // "PICK Multivalue DB",
    // "Pickup",
    // "Pinterest",
    // "Piping",
    // "PLC / SCADA",
    // "Plesk",
    // "Plugin",
    // "Plumbing",
    // "Poet",
    // "Poetry",
    // "Polish",
    // "Portuguese",
    // "Portuguese (Brazil)",
    // "Post-Production",
    // "Poster Design",
    // "PostgreSQL",
    // "Powerpoint",
    // "Powershell",
    // "Pre-production",
    // "Presentations",
    // "Press Releases",
    // "Prestashop",
    // "Prezi",
    // "Print",
    // "Procurement",
    // "Product Descriptions",
    // "Product Design",
    // "Product Management",
    // "Product Sourcing",
    // "Project Management",
    // "Project Scheduling",
    // "Prolog",
    // "Proofreading",
    // "Property Development",
    // "Property Law",
    // "Property Management",
    // "Proposal/Bid Writing",
    // "Protoshare",
    // "PSD to HTML",
    // "PSD2CMS",
    // "Psychology",
    // "Public Relations",
    // "Publishing",
    // "Punjabi",
    // "Puppet",
    // "Python",
    // "QlikView",
    // "Qualtrics Survey Platform",
    // "Quantum",
    // "QuarkXPress",
    // "QuickBase",
    // "R Programming Language",
    // "RapidWeaver",
    // "Raspberry Pi",
    // "Ray-tracing",
    // "React.js",
    // "Real Estate",
    // "REALbasic",
    // "Recruitment",
    // "Red Hat",
    // "Redis",
    // "Redshift",
    // "Regular Expressions",
    // "Remote Sensing",
    // "Removalist",
    // "Renewable Energy Design",
    // "Report Writing",
    // "Research",
    // "RESTful",
    // "Resumes",
    // "Reviews",
    // "Risk Management",
    // "Robotics",
    // "Rocket Engine",
    // "Romanian",
    // "Roofing",
    // "RTOS",
    // "Ruby",
    // "Ruby on Rails",
    // "Russian",
    // "RWD",
    // "Sales",
    // "Salesforce App Development",
    // "Salesforce.com",
    // "Samsung",
    // "Samsung Accessory SDK",
    // "SAP",
    // "SAS",
    // "Scala",
    // "Scheme",
    // "Scientific Research",
    // "Screenwriting",
    // "Script Install",
    // "Scrum",
    // "Scrum Development",
    // "Sculpturing",
    // "Search Engine Marketing",
    // "Sencha / YahooUI",
    // "SEO",
    // "Serbian",
    // "Sewing",
    // "Sharepoint",
    // "Shell Script",
    // "Shopify",
    // "Shopify Templates",
    // "Shopping",
    // "Shopping Carts",
    // "Short Stories",
    // "Siebel",
    // "Sign Design",
    // "Silverlight",
    // "Simplified Chinese (China)",
    // "Slogans",
    // "Slovakian",
    // "Slovenian",
    // "Smarty PHP",
    // "Snapchat",
    // "Social Engine",
    // "Social Media Marketing",
    // "Social Networking",
    // "Socket IO",
    // "Software Architecture",
    // "Software Development",
    // "Software Testing",
    // "Solaris",
    // "Solidworks",
    // "Sound Design",
    // "Spanish",
    // "Spanish (Spain)",
    // "Spark",
    // "Speech Writing",
    // "Sphinx",
    // "Splunk",
    // "Sports",
    // "SPSS Statistics",
    // "SQL",
    // "SQLite",
    // "Squarespace",
    // "Squid Cache",
    // "Startups",
    // "Stationery Design",
    // "Statistical Analysis",
    // "Statistics",
    // "Steam API",
    // "Sticker Design",
    // "Storage Area Networks",
    // "Storyboard",
    // "Stripe",
    // "Structural Engineering",
    // "Subversion",
    // "SugarCRM",
    // "Supplier Sourcing",
    // "Surfboard Design",
    // "Swedish",
    // "Swift",
    // "Symbian",
    // "Symfony PHP",
    // "System Admin",
    // "T-Shirts",
    // "Tableau",
    // "Tally Definition Language",
    // "Tamil",
    // "TaoBao API",
    // "Tattoo Design",
    // "Tax",
    // "Tax Law",
    // "Technical Support",
    // "Technical Writing",
    // "Tekla Structures",
    // "Telecommunications Engineering",
    // "Telemarketing",
    // "Telephone Handling",
    // "Telugu",
    // "Templates",
    // "Test Automation",
    // "Testing / QA",
    // "TestStand",
    // "Textile Engineering",
    // "Thai",
    // "Tibco Spotfire",
    // "Tiling",
    // "Time Management",
    // "Titanium",
    // "Tizen SDK for Wearables",
    // "Traditional Chinese (Hong Kong)",
    // "Traditional Chinese (Taiwan)",
    // "Training",
    // "Transcription",
    // "Translation",
    // "Travel Writing",
    // "Troubleshooting",
    // "Tumblr",
    // "Turkish",
    // "Twitter",
    // "Typescript",
    // "TYPO3",
    // "Typography",
    // "Ubuntu",
    // "Ukrainian",
    // "Umbraco",
    // "UML Design",
    // "Unified Communications",
    // "Unit4 Business World",
    // "Unity 3D",
    // "UNIX",
    // "Usability Testing",
    // "User Experience Design",
    // "User Interface / IA",
    // "User Interface Design",
    // "Vagrant",
    // "Valuation & Appraisal",
    // "VB.NET",
    // "vBulletin",
    // "Vectorization",
    // "Veeam",
    // "Vehicle Signage",
    // "Vendor Management",
    // "Verilog / VHDL",
    // "Videography",
    // "Vietnamese",
    // "Virtual Assistant",
    // "Virtual Worlds",
    // "Virtuemart",
    // "Virtuozzo",
    // "Visual Basic",
    // "Visual Basic for Apps",
    // "Visual Foxpro",
    // "Visual Merchandising",
    // "Visual Studio",
    // "VMware",
    // "Voice Artist",
    // "Voice Talent",
    // "VoIP",
    // "Volunteer",
    // "VPS",
    // "VRML",
    // "Vue.js",
    // "WAMP",
    // "WatchKit",
    // "Waterproofing",
    // "Web Hosting",
    // "Web Scraping",
    // "Web Security",
    // "Web Services",
    // "Web Testing",
    // "WebApp Pentesting",
    // "Website Design",
    // "Website Management",
    // "Website Testing",
    // "Weddings",
    // "Welding",
    // "Whiteboard Animation",
    // "Wikipedia",
    // "Windows 8",
    // "Windows API",
    // "Windows Desktop",
    // "Windows Mobile",
    // "Windows Phone",
    // "Windows Server",
    // "WinRT",
    // "Wix",
    // "Word",
    // "Word Processing",
    // "WordPress",
    // "Workflow",
    // "WPF",
    // "Writers",
    // "Writing",
    // "WYSIWYG",
    // "x86/x64 Assembler",
    // "Xamarin",
    // "Xero",
    // "XML",
    // "XMPP",
    // "Xoops",
    // "XPages",
    "XPath",
    "XQuery",
    "XSLT",
    "Yii",
    "YouTube",
    "Zen Cart",
    "Zend",
    "Zendesk",
    "Zoho",
    "Zookeeper",
    "Zope",
    "Zurb Foundation",
    "ZWave"
};

        await db.Skills.AddRangeAsync(skills.Select(s=>new Skill{Name = s}));
        await db.SaveChangesAsync();
    }
}
