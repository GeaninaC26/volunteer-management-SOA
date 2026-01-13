#!/bin/bash

# Script to add mock data to the volunteer management system via the backend API
# This script populates: locations, recruitmentFormTemplates, interviewTemplates, recruitmentCampaigns, and volunteers

BASE_URL="http://api-gateway:8080/api"
JWT_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImdpYW5pbmFjbGlwYUBnbWFpbC5jb20iLCJzdWIiOiJnaWFuaW5hY2xpcGFAZ21haWwuY29tIiwianRpIjoiOTAwMTBkY2YiLCJlbWFpbGFkZHJlc3MiOiJnaWFuaW5hY2xpcGFAZ21haWwuY29tIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo4MDgwIiwibmJmIjoxNzY3NzM0NjMyLCJleHAiOjE3NzU1MTA2MzIsImlhdCI6MTc2NzczNDYzMywiaXNzIjoiZG90bmV0LXVzZXItand0cyJ9.zW-gHmqY94wMCOg5QcnQaZdUmfzQU3X9HXwZX9Z8SJw"

echo "🚀 Starting to populate mock data..."

# Color codes for better output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Function to check if data exists by searching for it
check_exists() {
    local endpoint=$1
    local search_field=$2
    local search_value=$3
    
    response=$(curl -s -H "Authorization: Bearer $JWT_TOKEN" "$BASE_URL$endpoint")
    
    # Check if the response contains the search value
    if echo "$response" | grep -q "\"$search_field\":\"$search_value\"" || \
       echo "$response" | grep -q "\"$search_field\":$search_value"; then
        # Extract ID from the matching item
        # This is a simple extraction - assumes ID comes before the search field
        id=$(echo "$response" | grep -B 5 "\"$search_field\"" | grep -o "\"id\":[0-9]*" | head -1 | grep -o "[0-9]*")
        echo "$id"
    else
        echo "0"
    fi
}

# Function to make POST requests and capture the ID from response
post_and_get_id() {
    local endpoint=$1
    local data=$2
    local description=$3
    local check_field=$4
    local check_value=$5
    
    # Check if item already exists
    if [ -n "$check_field" ] && [ -n "$check_value" ]; then
        existing_id=$(check_exists "$endpoint" "$check_field" "$check_value")
        if [ "$existing_id" != "0" ]; then
            echo -e "${YELLOW}⊙ $description already exists (ID: $existing_id)${NC}" >&2
            echo "$existing_id"
            return
        fi
    fi
    
    echo -e "${YELLOW}Creating $description...${NC}" >&2
    
    http_response=$(curl -s -w "\n%{http_code}" -X POST \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $JWT_TOKEN" \
        -d "$data" \
        "$BASE_URL$endpoint")
    
    http_code=$(echo "$http_response" | tail -n1)
    response=$(echo "$http_response" | head -n-1)
    
    if [ "$http_code" -eq 200 ] && [ -n "$response" ] && [ "$response" != "0" ]; then
        echo -e "${GREEN}✓ Created $description (ID: $response, HTTP: $http_code)${NC}" >&2
        echo "$response"
    else
        echo -e "${RED}✗ Failed to create $description (HTTP: $http_code)${NC}" >&2
        if [ ${#response} -lt 500 ]; then
            echo -e "${RED}Response: $response${NC}" >&2
        else
            echo -e "${RED}Response: ${response:0:200}...${NC}" >&2
        fi
        echo "0"
    fi
}

# Wait for backend to be ready
echo "⏳ Waiting for backend to be ready..."
max_attempts=30
attempt=0

while [ $attempt -lt $max_attempts ]; do
    if curl -s "$BASE_URL/locations" > /dev/null 2>&1; then
        echo -e "${GREEN}✓ Backend is ready!${NC}"
        break
    fi
    attempt=$((attempt + 1))
    echo "Waiting... (attempt $attempt/$max_attempts)"
    sleep 2
done

if [ $attempt -eq $max_attempts ]; then
    echo -e "${RED}✗ Backend did not start in time. Exiting.${NC}"
    exit 1
fi

echo ""
echo "📍 Creating Locations..."
echo "========================"

location1_id=$(post_and_get_id "/locations" '{
    "name": "Main Campus",
    "address": "Strada Universității 1, Cluj-Napoca, Romania"
}' "Location: Main Campus" "name" "Main Campus")

location2_id=$(post_and_get_id "/locations" '{
    "name": "Student Center",
    "address": "Strada Clinicilor 5-7, Cluj-Napoca, Romania"
}' "Location: Student Center" "name" "Student Center")

location3_id=$(post_and_get_id "/locations" '{
    "name": "Hasdeu Residence",
    "address": "Strada Mihail Kogălniceanu 59, Cluj-Napoca, Romania"
}' "Location: Hasdeu Residence" "name" "Hasdeu Residence")

location4_id=$(post_and_get_id "/locations" '{
    "name": "Faculty of Computer Science",
    "address": "Strada Mihail Kogălniceanu 1, Cluj-Napoca, Romania"
}' "Location: Faculty of Computer Science" "name" "Faculty of Computer Science")

echo ""
echo "📋 Creating Recruitment Form Templates..."
echo "=========================================="

form_template1_id=$(post_and_get_id "/recruitment_form_templates" '{
    "name": "General Volunteer Application 2026",
    "questions": [
        "Why do you want to become a volunteer?",
        "What skills do you bring to our organization?",
        "Do you have any previous volunteering experience?",
        "What department are you most interested in?",
        "How many hours per week can you commit?",
        "Tell us about a time you worked in a team.",
        "What are your hobbies and interests?"
    ]
}' "Form Template: General Volunteer Application 2026" "name" "General Volunteer Application 2026")

form_template2_id=$(post_and_get_id "/recruitment_form_templates" '{
    "name": "Events Team Application",
    "questions": [
        "Have you organized or helped organize events before?",
        "What type of events are you most interested in?",
        "Are you comfortable working under pressure?",
        "Can you work flexible hours including evenings and weekends?",
        "Do you have experience with event planning software?"
    ]
}' "Form Template: Events Team Application" "name" "Events Team Application")

form_template3_id=$(post_and_get_id "/recruitment_form_templates" '{
    "name": "Tech Team Application",
    "questions": [
        "What programming languages are you familiar with?",
        "Do you have experience with web development?",
        "Have you contributed to open source projects?",
        "What is your experience with databases?",
        "Are you familiar with version control systems like Git?"
    ]
}' "Form Template: Tech Team Application" "name" "Tech Team Application")

echo ""
echo "🎤 Creating Interview Templates..."
echo "=================================="

interview_template1_id=$(post_and_get_id "/interview_templates" '{
    "name": "Standard Interview 2026",
    "questions": [
        "Tell us about yourself.",
        "Why did you choose to volunteer with us?",
        "What are your strengths and weaknesses?",
        "Describe a challenging situation and how you handled it.",
        "What are your expectations from this volunteering experience?",
        "How do you handle conflicts in a team?",
        "What would you like to achieve during your time as a volunteer?"
    ],
    "duration": 30
}' "Interview Template: Standard Interview 2026" "name" "Standard Interview 2026")

interview_template2_id=$(post_and_get_id "/interview_templates" '{
    "name": "Leadership Interview",
    "questions": [
        "What does leadership mean to you?",
        "Give an example of when you led a team.",
        "How do you motivate team members?",
        "How do you handle disagreements within your team?",
        "What is your management style?"
    ],
    "duration": 45
}' "Interview Template: Leadership Interview" "name" "Leadership Interview")

interview_template3_id=$(post_and_get_id "/interview_templates" '{
    "name": "Quick Assessment",
    "questions": [
        "Why are you interested in this role?",
        "What can you contribute to our team?",
        "When can you start?"
    ],
    "duration": 15
}' "Interview Template: Quick Assessment" "name" "Quick Assessment")

echo ""
echo "🎯 Creating Recruitment Campaigns..."
echo "====================================="

# Calculate dates
current_year=$(date +%Y)
current_month=$(date +%m)

campaign1_id=$(post_and_get_id "/campaigns" "{
    \"name\": \"spring_recruitment_2026\",
    \"startDate\": \"2026-01-01\",
    \"endDate\": \"2026-03-31\",
    \"interviewTemplateId\": $interview_template1_id,
    \"recruitmentFormTemplateId\": $form_template1_id
}" "Campaign: spring_recruitment_2026" "name" "spring_recruitment_2026")

campaign2_id=$(post_and_get_id "/campaigns" "{
    \"name\": \"events_team_expansion\",
    \"startDate\": \"2026-01-01\",
    \"endDate\": \"2026-02-28\",
    \"interviewTemplateId\": $interview_template2_id,
    \"recruitmentFormTemplateId\": $form_template2_id
}" "Campaign: events_team_expansion" "name" "events_team_expansion")

campaign3_id=$(post_and_get_id "/campaigns" "{
    \"name\": \"tech_volunteers_q1_2026\",
    \"startDate\": \"2026-01-01\",
    \"endDate\": \"2026-03-31\",
    \"interviewTemplateId\": $interview_template3_id,
    \"recruitmentFormTemplateId\": $form_template3_id
}" "Campaign: tech_volunteers_q1_2026" "name" "tech_volunteers_q1_2026")

campaign4_id=$(post_and_get_id "/campaigns" "{
    \"name\": \"summer_festival_team\",
    \"startDate\": \"2026-01-01\",
    \"endDate\": \"2026-05-31\",
    \"interviewTemplateId\": $interview_template1_id,
    \"recruitmentFormTemplateId\": $form_template2_id
}" "Campaign: summer_festival_team" "name" "summer_festival_team")

echo ""
echo "👥 Creating Volunteers..."
echo "========================="

volunteer1_id=$(post_and_get_id "/volunteers" '{
    "firstName": "Maria",
    "lastName": "Popescu",
    "personalEmail": "maria.popescu@example.com",
    "phone": "0740123456",
    "email": "maria.popescu@volunteer.org",
    "volunteerStatus": "Active",
    "department": "Events"
}' "Volunteer: Maria Popescu" "personalEmail" "maria.popescu@example.com")

volunteer2_id=$(post_and_get_id "/volunteers" '{
    "firstName": "Ion",
    "lastName": "Ionescu",
    "personalEmail": "ion.ionescu@example.com",
    "phone": "0741234567",
    "email": "ion.ionescu@volunteer.org",
    "volunteerStatus": "Active",
    "department": "HumanResources"
}' "Volunteer: Ion Ionescu" "personalEmail" "ion.ionescu@example.com")

volunteer3_id=$(post_and_get_id "/volunteers" '{
    "firstName": "Ana",
    "lastName": "Marinescu",
    "personalEmail": "ana.marinescu@example.com",
    "phone": "0742345678",
    "volunteerStatus": "Active",
    "department": "ImageAndPR"
}' "Volunteer: Ana Marinescu" "personalEmail" "ana.marinescu@example.com")

volunteer4_id=$(post_and_get_id "/volunteers" '{
    "firstName": "Andrei",
    "lastName": "Constantin",
    "personalEmail": "andrei.constantin@example.com",
    "phone": "0743456789",
    "email": "andrei.constantin@volunteer.org",
    "volunteerStatus": "Active",
    "department": "ExternalRelations"
}' "Volunteer: Andrei Constantin" "personalEmail" "andrei.constantin@example.com")

volunteer5_id=$(post_and_get_id "/volunteers" '{
    "firstName": "Elena",
    "lastName": "Georgescu",
    "personalEmail": "elena.georgescu@example.com",
    "phone": "0744567890",
    "volunteerStatus": "Inactive",
    "department": "Events"
}' "Volunteer: Elena Georgescu" "personalEmail" "elena.georgescu@example.com")

volunteer6_id=$(post_and_get_id "/volunteers" '{
    "firstName": "Mihai",
    "lastName": "Stancu",
    "personalEmail": "mihai.stancu@example.com",
    "phone": "0745678901",
    "email": "mihai.stancu@volunteer.org",
    "volunteerStatus": "Active",
    "department": "Events"
}' "Volunteer: Mihai Stancu" "personalEmail" "mihai.stancu@example.com")

volunteer7_id=$(post_and_get_id "/volunteers" '{
    "firstName": "Alexandra",
    "lastName": "Dumitrescu",
    "personalEmail": "alexandra.dumitrescu@example.com",
    "phone": "0746789012",
    "email": "alexandra.dumitrescu@volunteer.org",
    "volunteerStatus": "Active",
    "department": "HumanResources"
}' "Volunteer: Alexandra Dumitrescu" "personalEmail" "alexandra.dumitrescu@example.com")

volunteer8_id=$(post_and_get_id "/volunteers" '{
    "firstName": "Cristian",
    "lastName": "Radu",
    "personalEmail": "cristian.radu@example.com",
    "phone": "0747890123",
    "volunteerStatus": "Active",
    "department": "ImageAndPR"
}' "Volunteer: Cristian Radu" "personalEmail" "cristian.radu@example.com")

echo ""
echo "👨‍🎓 Creating Candidates..."
echo "=========================="

# Candidates for Spring Recruitment 2026
if [ "$campaign1_id" != "0" ]; then
    candidate1_id=$(post_and_get_id "/campaigns/${campaign1_id}/candidates" "{
        \"firstName\": \"Ioana\",
        \"lastName\": \"Vasilescu\",
        \"personalEmail\": \"ioana.vasilescu@student.utcluj.ro\",
        \"phone\": \"0750123456\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Ioana Vasilescu (Spring Recruitment)" "personalEmail" "ioana.vasilescu@student.utcluj.ro")

    candidate2_id=$(post_and_get_id "/campaigns/${campaign1_id}/candidates" "{
        \"firstName\": \"George\",
        \"lastName\": \"Popa\",
        \"personalEmail\": \"george.popa@student.utcluj.ro\",
        \"phone\": \"0751234567\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: George Popa (Spring Recruitment)" "personalEmail" "george.popa@student.utcluj.ro")

    candidate3_id=$(post_and_get_id "/campaigns/${campaign1_id}/candidates" "{
        \"firstName\": \"Diana\",
        \"lastName\": \"Munteanu\",
        \"personalEmail\": \"diana.munteanu@student.utcluj.ro\",
        \"phone\": \"0752345678\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Diana Munteanu (Spring Recruitment)" "personalEmail" "diana.munteanu@student.utcluj.ro")

    candidate4_id=$(post_and_get_id "/campaigns/${campaign1_id}/candidates" "{
        \"firstName\": \"Vlad\",
        \"lastName\": \"Tanase\",
        \"personalEmail\": \"vlad.tanase@student.utcluj.ro\",
        \"phone\": \"0753456789\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Vlad Tanase (Spring Recruitment)" "personalEmail" "vlad.tanase@student.utcluj.ro")
fi

# Candidates for Events Team Expansion
if [ "$campaign2_id" != "0" ]; then
    candidate5_id=$(post_and_get_id "/campaigns/${campaign2_id}/candidates" "{
        \"firstName\": \"Andreea\",
        \"lastName\": \"Serban\",
        \"personalEmail\": \"andreea.serban@student.utcluj.ro\",
        \"phone\": \"0754567890\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Andreea Serban (Events Team)" "personalEmail" "andreea.serban@student.utcluj.ro")

    candidate6_id=$(post_and_get_id "/campaigns/${campaign2_id}/candidates" "{
        \"firstName\": \"Radu\",
        \"lastName\": \"Marin\",
        \"personalEmail\": \"radu.marin@student.utcluj.ro\",
        \"phone\": \"0755678901\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Radu Marin (Events Team)" "personalEmail" "radu.marin@student.utcluj.ro")

    candidate7_id=$(post_and_get_id "/campaigns/${campaign2_id}/candidates" "{
        \"firstName\": \"Carmen\",
        \"lastName\": \"Lungu\",
        \"personalEmail\": \"carmen.lungu@student.utcluj.ro\",
        \"phone\": \"0756789012\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Carmen Lungu (Events Team)" "personalEmail" "carmen.lungu@student.utcluj.ro")
fi

# Candidates for Tech Volunteers Q1 2026
if [ "$campaign3_id" != "0" ]; then
    candidate8_id=$(post_and_get_id "/campaigns/${campaign3_id}/candidates" "{
        \"firstName\": \"Stefan\",
        \"lastName\": \"Albu\",
        \"personalEmail\": \"stefan.albu@student.utcluj.ro\",
        \"phone\": \"0757890123\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Stefan Albu (Tech Team)" "personalEmail" "stefan.albu@student.utcluj.ro")

    candidate9_id=$(post_and_get_id "/campaigns/${campaign3_id}/candidates" "{
        \"firstName\": \"Laura\",
        \"lastName\": \"Ungureanu\",
        \"personalEmail\": \"laura.ungureanu@student.utcluj.ro\",
        \"phone\": \"0758901234\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Laura Ungureanu (Tech Team)" "personalEmail" "laura.ungureanu@student.utcluj.ro")

    candidate10_id=$(post_and_get_id "/campaigns/${campaign3_id}/candidates" "{
        \"firstName\": \"Bogdan\",
        \"lastName\": \"Nica\",
        \"personalEmail\": \"bogdan.nica@student.utcluj.ro\",
        \"phone\": \"0759012345\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Bogdan Nica (Tech Team)" "personalEmail" "bogdan.nica@student.utcluj.ro")

    candidate11_id=$(post_and_get_id "/campaigns/${campaign3_id}/candidates" "{
        \"firstName\": \"Simona\",
        \"lastName\": \"Cretu\",
        \"personalEmail\": \"simona.cretu@student.utcluj.ro\",
        \"phone\": \"0750234567\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Simona Cretu (Tech Team)" "personalEmail" "simona.cretu@student.utcluj.ro")
fi

# Candidates for Summer Festival Team
if [ "$campaign4_id" != "0" ]; then
    candidate12_id=$(post_and_get_id "/campaigns/${campaign4_id}/candidates" "{
        \"firstName\": \"Adrian\",
        \"lastName\": \"Badea\",
        \"personalEmail\": \"adrian.badea@student.utcluj.ro\",
        \"phone\": \"0751345678\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Adrian Badea (Summer Festival)" "personalEmail" "adrian.badea@student.utcluj.ro")

    candidate13_id=$(post_and_get_id "/campaigns/${campaign4_id}/candidates" "{
        \"firstName\": \"Monica\",
        \"lastName\": \"Dobre\",
        \"personalEmail\": \"monica.dobre@student.utcluj.ro\",
        \"phone\": \"0752456789\",
        \"recruitingStatus\": \"Open\"
    }" "Candidate: Monica Dobre (Summer Festival)" "personalEmail" "monica.dobre@student.utcluj.ro")
fi

echo ""
echo "🤝 Assigning Volunteers to Campaigns..."
echo "========================================"

# Function to assign volunteer to campaign
assign_volunteer() {
    local campaign_id=$1
    local volunteer_id=$2
    local description=$3
    
    if [ "$campaign_id" = "0" ] || [ "$volunteer_id" = "0" ]; then
        echo -e "${YELLOW}⊙ Skipping $description (missing IDs)${NC}"
        return
    fi
    
    echo -e "${YELLOW}Assigning $description...${NC}"
    
    http_code=$(curl -s -w "%{http_code}" -o /dev/null -X POST \
        -H "Authorization: Bearer $JWT_TOKEN" \
        "$BASE_URL/campaigns/${campaign_id}/volunteers/?id=${volunteer_id}")
    
    if [ "$http_code" -eq 200 ]; then
        echo -e "${GREEN}✓ Assigned $description (HTTP: $http_code)${NC}"
    else
        echo -e "${RED}✗ Failed to assign $description (HTTP: $http_code)${NC}"
    fi
}

# Assign volunteers to Spring Recruitment 2026
if [ "$campaign1_id" != "0" ]; then
    assign_volunteer "$campaign1_id" "$volunteer2_id" "Ion Ionescu (HR) to Spring Recruitment"
    assign_volunteer "$campaign1_id" "$volunteer7_id" "Alexandra Dumitrescu (HR) to Spring Recruitment"
    assign_volunteer "$campaign1_id" "$volunteer1_id" "Maria Popescu (Events) to Spring Recruitment"
fi

# Assign volunteers to Events Team Expansion
if [ "$campaign2_id" != "0" ]; then
    assign_volunteer "$campaign2_id" "$volunteer1_id" "Maria Popescu (Events) to Events Team Expansion"
    assign_volunteer "$campaign2_id" "$volunteer6_id" "Mihai Stancu (Events) to Events Team Expansion"
    assign_volunteer "$campaign2_id" "$volunteer3_id" "Ana Marinescu (PR) to Events Team Expansion"
fi

# Assign volunteers to Tech Volunteers Q1 2026
if [ "$campaign3_id" != "0" ]; then
    assign_volunteer "$campaign3_id" "$volunteer2_id" "Ion Ionescu (HR) to Tech Volunteers"
    assign_volunteer "$campaign3_id" "$volunteer4_id" "Andrei Constantin (External Relations) to Tech Volunteers"
fi

# Assign volunteers to Summer Festival Team
if [ "$campaign4_id" != "0" ]; then
    assign_volunteer "$campaign4_id" "$volunteer1_id" "Maria Popescu (Events) to Summer Festival"
    assign_volunteer "$campaign4_id" "$volunteer6_id" "Mihai Stancu (Events) to Summer Festival"
    assign_volunteer "$campaign4_id" "$volunteer3_id" "Ana Marinescu (PR) to Summer Festival"
    assign_volunteer "$campaign4_id" "$volunteer8_id" "Cristian Radu (PR) to Summer Festival"
fi

echo ""
echo -e "${GREEN}✅ Mock data creation complete!${NC}"
echo ""

# Verify data was actually created by querying the API
echo -e "${YELLOW}🔍 Verifying data in database...${NC}"
echo ""

verify_count() {
    local endpoint=$1
    local entity_name=$2
    
    response=$(curl -s -H "Authorization: Bearer $JWT_TOKEN" "$BASE_URL$endpoint")
    count=$(echo "$response" | grep -o "\"id\"" | wc -l)
    
    if [ "$count" -gt 0 ]; then
        echo -e "${GREEN}✓ $entity_name: $count items found${NC}"
    else
        echo -e "${RED}✗ $entity_name: No items found!${NC}"
    fi
}

verify_count "/locations" "Locations"
verify_count "/recruitment_form_templates" "Recruitment Form Templates"
verify_count "/interview_templates" "Interview Templates"
verify_count "/campaigns" "Recruitment Campaigns"
verify_count "/volunteers" "Volunteers"

# Verify candidates in each campaign
echo ""
echo "Candidates per campaign:"
if [ "$campaign1_id" != "0" ]; then
    verify_count "/campaigns/${campaign1_id}/candidates" "  └─ Spring Recruitment 2026"
fi
if [ "$campaign2_id" != "0" ]; then
    verify_count "/campaigns/${campaign2_id}/candidates" "  └─ Events Team Expansion"
fi
if [ "$campaign3_id" != "0" ]; then
    verify_count "/campaigns/${campaign3_id}/candidates" "  └─ Tech Volunteers Q1 2026"
fi
if [ "$campaign4_id" != "0" ]; then
    verify_count "/campaigns/${campaign4_id}/candidates" "  └─ Summer Festival Team"
fi

# Verify volunteers in each campaign
echo ""
echo "Volunteers per campaign:"
if [ "$campaign1_id" != "0" ]; then
    verify_count "/campaigns/${campaign1_id}/volunteers" "  └─ Spring Recruitment 2026"
fi
if [ "$campaign2_id" != "0" ]; then
    verify_count "/campaigns/${campaign2_id}/volunteers" "  └─ Events Team Expansion"
fi
if [ "$campaign3_id" != "0" ]; then
    verify_count "/campaigns/${campaign3_id}/volunteers" "  └─ Tech Volunteers Q1 2026"
fi
if [ "$campaign4_id" != "0" ]; then
    verify_count "/campaigns/${campaign4_id}/volunteers" "  └─ Summer Festival Team"
fi

echo ""
echo "Summary:"
echo "--------"
echo "Locations: 4"
echo "Recruitment Form Templates: 3"
echo "Interview Templates: 3"
echo "Recruitment Campaigns: 4"
echo "Volunteers: 8"
echo "Candidates: ~13 (distributed across campaigns)"
echo "Campaign-Volunteer Assignments: Multiple volunteers assigned to manage campaigns"
echo ""
echo -e "${GREEN}🎉 All done! Your database is now populated with mock data.${NC}"
