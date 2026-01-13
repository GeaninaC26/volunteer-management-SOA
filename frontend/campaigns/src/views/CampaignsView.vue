eslint-disable vue/require-v-for-key
<template>
  <div class="recruitment-campaign-container">
    <div class="display-campaigns">
      <!-- Campaigns table -->
      <div class="table-container">
        <table class="pretty-table">
          <thead>
            <tr class="table-head">
              <th>Campaign name</th>
              <th>Start date</th>
              <th>End date</th>
              <th></th>
              <th></th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="campaign in campaigns" :key="campaign.id">
              <td class="bold">
                <router-link :to="`/campaigns/${campaign.name}`">
                  {{ campaign.name }}
                </router-link>
              </td>
              <td>{{ campaign.startDate }}</td>
              <td>{{ campaign.endDate }}</td>
              <td>
                <button
                  class="primary-btn"
                  data-bs-toggle="modal"
                  data-bs-target="#editCampaignModal"
                  aria-controls="editCampaignModal"
                  @click="selectCampaign(campaign)"
                >
                  Edit campaign
                </button>
              </td>
              <td>
                <router-link
                  :to="`/schedule_interviews/${campaign.name}`"
                  class="primary-btn"
                >
                  Schedule
                </router-link>
              </td>
              <td>
                <router-link
                  :to="`/register/${campaign.name}`"
                  class="primary-btn"
                >
                  Registration form
                </router-link>
              </td>
            </tr>
            <tr>
              <!-- <td colspan="4" style="text-align: center">

              </td> -->
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div
      class="recruiting-campaign-buttons"
      style="display: flex; gap: 32px; justify-content: center; padding: 1%"
    >
      <button
        class="primary-btn"
        data-bs-toggle="modal"
        data-bs-target="#addCampaignModal"
        aria-controls="addCampaignModal"
      >
        Create a recruiting campaign
      </button>
      <button
        class="primary-btn"
        data-bs-toggle="modal"
        data-bs-target="#addLocationModal"
        aria-controls="addLocationModal"
      >
        Create interview location
      </button>
      <button
        class="primary-btn"
        data-bs-toggle="modal"
        data-bs-target="#addTemplateModal"
        aria-controls="addTemplateModal"
      >
        Create interview template
      </button>
    </div>
    <div class="display-people">
      <!-- Locations table -->
      <div class="table-container">
        <table class="pretty-table">
          <thead>
            <tr class="table-head">
              <th colspan="3">Locations</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="location in locations" :key="location.id">
              <td>{{ location.name }}</td>
              <td class="bold">{{ location.address }}</td>
              <td>
                <!-- <button class="danger-btn" v-on:click="removeVolunteer(volunteer.id)">X</button> -->
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Interview templates table -->
      <div class="table-container">
        <table class="pretty-table">
          <thead>
            <tr>
              <th colspan="100%">Interview templates</th>
            </tr>
          </thead>
          <tr v-for="template in interviewTemplates" :key="template.id">
            <td>{{ template.name }}</td>
            <td>{{ template.duration }}</td>
            <td></td>
            <td>
              <button
                class="primary-btn"
                data-bs-toggle="modal"
                data-bs-target="#seeQuestionsInterviewTemplateModal"
                aria-controls="seeQuestionsInterviewTemplateModal"
                @click="selectInterviewTemplate(template)"
              >
                See questions
              </button>
            </td>
          </tr>
        </table>
      </div>
    </div>
<!-- Recruitment Form Table -->
          <div class="table-container">
        <table class="pretty-table">
          <thead>
            <tr>
              <th colspan="100%">Recruitment form templates</th>
            </tr>
          </thead>
          <tr v-for="template in recruitmentFormTemplates" :key="template.id">
            <td>{{ template.name }}</td>
            <td></td>
            <td>
              <button
                class="primary-btn"
                data-bs-toggle="modal"
                data-bs-target="#selectedRecruitmentFormTemplate"
                aria-controls="selectedRecruitmentFormTemplate"
                @click="selectRecruitmentFormTemplate(template)"
              >
                See questions
              </button>
            </td>
          </tr>
        </table>
      </div>
  </div>

  <!-- Modal Add Campaigns -->
  <div
    class="modal fade"
    id="addCampaignModal"
    tabindex="-1"
    role="dialog"
    aria-labelledby="addCampaignModal"
    aria-hidden="true"
  >
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="addCampaignModalLabel">Create a recruiting campaign.</h5>
        </div>
        <div class="modal-body">
          <h5 style="color: red">Important:</h5>
          <h5>The recommended name for campaigns is {seasonYEAR} (ex: spring2023, autumn2024)</h5>
          <form>
            <div class="form-item">
              <h3>Campaign Name</h3>
              <input v-model="newCampaign.name" placeholder="Campaign Name" required />
            </div>
            <div class="form-item">
              <h3>Start date</h3>
              <DatePicker
                v-model="newCampaign.startDate"
                placeholder="Start date"
                inputId="date"
                dateFormat="yy-mm-dd"
                showIcon
              />
            </div>
            <div class="form-item">
              <h3>End date</h3>
              <DatePicker
                v-model="newCampaign.endDate"
                placeholder="End date"
                inputId="date"
                dateFormat="yy-mm-dd"
                showIcon
              />
            </div>
            <div class="form-item">
              <h3>Interview Template</h3>
              <select v-model="selectedTemplateCampaign" required>
                <option disabled value="">Select template</option>
                <option v-for="template in interviewTemplates" :key="template" :value="template">
                  {{ template.name }}
                </option>
              </select>
            </div>
              <div class="form-item">
              <h3>Recruitment Form Template</h3>
              <select v-model="selectedRFTemplateCampaign" required>
                <option disabled value="">Select template</option>
                <option v-for="template in recruitmentFormTemplates" :key="template" :value="template">
                  {{ template.name }}
                </option>
              </select>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button
            type="button"
            class="btn btn-secondary"
            @click="addCampaign()"
            data-bs-dismiss="modal"
          >
            Create
          </button>
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>
  <!-- Modal edit campaign -->
  <div
    class="modal fade"
    id="editCampaignModal"
    tabindex="-1"
    role="dialog"
    aria-labelledby="editCampaignModal"
    aria-hidden="true"
  >
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="editCampaignModalLabel">Edit the recruiting campaign.</h5>
        </div>
        <div class="modal-body">
          <form>
            <div class="form-item">
              <h3>Campaign Name</h3>
              <input v-model="selectedCampaign.name" placeholder="Campaign Name" required />
            </div>
            <div class="form-item">
              <h3>Start date</h3>
              <DatePicker
                v-model="selectedCampaign.startDate"
                placeholder="Start date"
                inputId="date"
                dateFormat="yy-mm-dd"
                showIcon
              />
            </div>
            <div class="form-item">
              <h3>End date</h3>
              <DatePicker
                v-model="selectedCampaign.endDate"
                placeholder="End date"
                inputId="date"
                dateFormat="yy-mm-dd"
                showIcon
              />
            </div>
            <div class="form-item" style="padding: 10% 10% 0% 10%">
              <h3>Add Blocked Period</h3>
              <DatePicker
                v-model="blockedPeriod.start"
                placeholder="Start date"
                inputId="date"
                dateFormat="yy-mm-dd"
                show-time
                hourFormat="24"
                showIcon
              />

              <h3>Duration</h3>
              <DatePicker
                v-model="blockedPeriod.duration"
                placeholder="Duration"
                inputId="time"
                show-time
                hourFormat="24"
                timeOnly
              />
              <select v-model="locationBlocked" required>
                <option disabled value="">Select Location</option>
                <option v-for="location in locationsCampaign" :key="location" :value="location">
                  {{ location.name }}
                </option>
              </select>
              <button
                type="button"
                class="primary-btn"
                @click="addBlockedPeriod"
                style="margin: 10%"
              >
                Add a blocked period
              </button>
            </div>
            <div class="form-item" style="padding: 0% 10% 0% 10%">
              <h3>Add Locations</h3>
              <div>
                <ul>
                  <li v-for="loc in locationsCampaign" :key="loc.id">
                    {{ loc.name }}
                  </li>
                </ul>
              </div>
              <select v-model="locationForCampaign" required>
                <option disabled value="">Select Location</option>
                <option v-for="location in locations" :key="location" :value="location">
                  {{ location.name }}
                </option>
              </select>
              <button
                type="button"
                class="primary-btn"
                @click="addLocationToCampaign()"
                style="margin: 2% 10%"
              >
                Add a location
              </button>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button
            type="button"
            class="btn btn-secondary"
            @click="updateCampaign()"
            data-bs-dismiss="modal"
          >
            Update
          </button>

          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>

  <!-- Modal create location -->
  <div
    class="modal fade"
    id="addLocationModal"
    tabindex="-1"
    role="dialog"
    aria-labelledby="addLocationModal"
    aria-hidden="true"
  >
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="addLocationModalLabel">Create a location.</h5>
        </div>
        <div class="modal-body">
          <form>
            <div class="form-item">
              <h3>Name</h3>
              <input v-model="newLocation.name" placeholder="Name" required />
            </div>
            <div class="form-item">
              <h3>Address</h3>
              <input v-model="newLocation.address" placeholder="Address" required />
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button
            type="button"
            class="btn btn-secondary"
            @click="addLocation()"
            data-bs-dismiss="modal"
          >
            Create
          </button>
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>
  <!-- Modal create interview template -->
  <div
    class="modal fade"
    id="addTemplateModal"
    tabindex="-1"
    role="dialog"
    aria-labelledby="addTemplateModal"
    aria-hidden="true"
  >
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="addTemplateModalLabel">Create an interview template.</h5>
        </div>
        <div class="modal-body">
          <form>
            <div class="form-item">
              <h3>Name</h3>
              <input v-model="newTemplate.name" placeholder="Name" required />
            </div>
            <div class="form-item">
              <h3>Duration in minutes</h3>
              <input v-model="newTemplate.duration" placeholder="Duration" required />
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button
            type="button"
            class="btn btn-secondary"
            @click="addTemplate()"
            data-bs-dismiss="modal"
          >
            Create
          </button>
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>
  <!-- Modal add and see questions for interview template -->
  <div
    class="modal fade"
    id="seeQuestionsInterviewTemplateModal"
    tabindex="-1"
    role="dialog"
    aria-labelledby="seeQuestionsInterviewTemplateModal"
    aria-hidden="true"
  >
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="seeQuestionsInterviewTemplateModalLabel">
            Questions for {{ selectedTemplate.name }}
          </h5>
        </div>
        <div class="modal-body">
          <table class="pretty-table">
            <thead>
              <tr></tr>
            </thead>
            <tr v-for="q in selectedTemplate.questions" :key="q">
              <td>{{ q }}</td>
            </tr>
          </table>
          <!-- <br></br>
          <br></br> -->
          <form>
            <div class="form-item">
              <h3>Add a question to the template</h3>
              <input v-model="questionIT" placeholder="Question" required />
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="addQuestiontoInterviewTemplate()">
            Add question
          </button>
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>
  <!-- Modal add and see questions for Recruiting Form Questions -->
   <div
    class="modal fade"
    id="selectedRecruitmentFormTemplate"
    tabindex="-1"
    role="dialog"
    aria-labelledby="selectedRecruitmentFormTemplate"
    aria-hidden="true"
  >
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="selectedRecruitmentFormTemplate">
            Questions for {{ selectedRFTemplate.name }}
          </h5>
        </div>
        <div class="modal-body">
          <table class="pretty-table">
            <thead>
              <tr></tr>
            </thead>
            <tr v-for="q in selectedRFTemplate.questions" :key="q">
              <td>{{ q }}</td>
            </tr>
          </table>
          <!-- <br></br>
          <br></br> -->
          <form>
            <div class="form-item">
              <h3>Add a question to the template</h3>
              <input v-model="questionRFT" placeholder="Question" required />
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="addQuestionsToRecruitmentFormTemplate()">
            Add question
          </button>
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, reactive } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'
import DatePicker from 'primevue/datepicker'
import * as signalR from "@microsoft/signalr";

var campaigns = ref([])
var interviewTemplates = ref([])
var recruitmentFormTemplates = ref([])

var questionIT = ref('')
var questionRFT = ref('')

var locationForCampaign = ref('')
var locationBlocked = ref('')

const selectedTemplate = ref({
  name: '',
  questions: [],
  duration: '',
})
const selectedRFTemplate = ref({
  name: '',
  questions: [],
})
const blockedPeriod = ref({
  start: '',
  duration: '',
  locationId: '',
})

const selectedCampaign = ref({
  name: '',
  startDate: '',
  endDate: '',
})
const newCampaign = reactive({
  name: '',
  startDate: '',
  endDate: '',
})
const newLocation = reactive({
  name: '',
  address: '',
})
const newTemplate = reactive({
  name: '',
  questions: [],
  duration: '',
})
var locations = ref([])
var locationsCampaign = ref([])
var selectedTemplateCampaign = ref({ name: '', questions: [], duration: '' })
var selectedRFTemplateCampaign = ref({ name: '', questions: [] })

onMounted(async () => {
  const route = useRoute()
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notifications")
    .withAutomaticReconnect()
    .build();
  
  connection.onclose(async () => {
    await connection.invoke("LeavePageGroup", route.path);
  });
  connection.on("campaign_update", () => {
    console.log("campaign update received");
    axios.get(`/api/campaigns`).then((response) => {
      campaigns.value = response.data;
    });
  });
  connection.on("location_update", () => {
    console.log("location update received");
    axios.get(`/api/locations`).then((response) => {
      locations.value = response.data;
    });
  });
  connection.on("recruitment_form_template_update", () => {
    console.log("recruitment form template update received");
    axios.get(`/api/recruitment_form_templates`).then((response) => {
      recruitmentFormTemplates.value = response.data;
    });
  });
  connection.on("interview_template_update", () => {
    console.log("interview template update received");
    axios.get(`/api/interview_templates`).then((response) => {
      interviewTemplates.value = response.data;
    });
  });
  connection
    .start()
    .then(async () => {
      console.log("Connected to SignalR hub")
      await connection.invoke("JoinPageGroup", route.path);
    })
    .catch((err) => {
      console.error("SignalR connection error:", err);
    });


  var campaignsRes = await axios.get(`/api/campaigns`)
  var templateRes = await axios.get(`/api/interview_templates`)
  var locationsRes = await axios.get(`/api/locations`)
  var rftRes = await axios.get(`/api/recruitment_form_templates`)
  locations.value = locationsRes.data
  campaigns.value = campaignsRes.data
  interviewTemplates.value = templateRes.data
  recruitmentFormTemplates.value = rftRes.data
})

const selectCampaign = async (campaign) => {
  selectedCampaign.value = campaign
  var locationsRes = await axios.get(`/api/campaigns/${selectedCampaign.value.id}/locations`)
  locationsCampaign.value = locationsRes.data

}
const selectInterviewTemplate = (template) => {
  selectedTemplate.value = template
}
const selectRecruitmentFormTemplate = (template) => {
  selectedRFTemplate.value = template
}

const formatDate = (date) => {
  if (!date) return ''
  const d = new Date(date)
  const year = d.getFullYear()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  const year = d.getFullYear()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  const hours = String(d.getHours()).padStart(2, '0')
  const minutes = String(d.getMinutes()).padStart(2, '0')
  const seconds = String(d.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`
}
const formatDurationFromPicker = (date) => {
  if (!date) return ''
  const d = new Date(date)

  const hrs = String(d.getHours()).padStart(2, '0')
  const mins = String(d.getMinutes()).padStart(2, '0')
  return `${hrs}:${mins}:00`
}

const addLocationToCampaign = async () => {
  try {
    const response = await axios.post(
      `/api/campaigns/${selectedCampaign.value.id}/locations`,
       locationForCampaign.value.id,
       { headers: { "Content-Type": "application/json" } }
    )

    var locationsRes = await axios.get(`/api/campaigns/${selectedCampaign.value.id}/locations`)
    locationsCampaign.value = locationsRes.data

    if (response.status === 200) {
      alert('Location added.')
    } else {
      alert(response.data?.message || 'Error adding the location.')
    }
  } catch (err) {
    if (err.response && err.response.data && err.response.data.message) {
      alert(err.response.data)
    } else {
      alert('An error occurred.')
    }
    console.error(err)
  }
}

const addBlockedPeriod = async () => {
  try {
    console.log(blockedPeriod)
    blockedPeriod.value.locationId = locationBlocked.value.id
    blockedPeriod.value.start = formatDateTime(blockedPeriod.value.start)
    blockedPeriod.value.duration = formatDurationFromPicker(blockedPeriod.value.duration)
    console.log(selectedCampaign.value.id)
    const response = await axios.post(
      `/api/campaigns/${selectedCampaign.value.id}/blocked_periods`,
      {
        start: blockedPeriod.value.start,
        duration: blockedPeriod.value.duration,
        locationId: blockedPeriod.value.locationId,
      },
    )
    var campaignsRes = await axios.get(`/api/campaigns`)
    campaigns.value = campaignsRes.data
    if (response.status === 200) {
      alert('Blocked period added')
    } else {
      alert(response.data?.message || 'Error creating the campaign.')
    }
  } catch (err) {
    if (err.response && err.response.data && err.response.data.message) {
      alert(err.response.data)
    } else {
      alert('An error occurred.')
    }
    console.error(err)
  }
}

const addCampaign = async () => {
  try {
    newCampaign.startDate = formatDate(newCampaign.startDate)
    newCampaign.endDate = formatDate(newCampaign.endDate)
    newCampaign.interviewTemplateId = selectedTemplateCampaign.value.id
    newCampaign.recruitmentFormTemplateId = selectedRFTemplateCampaign.value.id
    console.log(newCampaign.interviewTemplate)
    const response = await axios.post('/api/campaigns', newCampaign)
    var campaignsRes = await axios.get(`/api/campaigns`)
    campaigns.value = campaignsRes.data
    if (response.status === 200) {
      alert('Recruitment campaign created')
    } else {
      alert(response.data?.message || 'Error creating the campaign.')
    }
  } catch (err) {
    if (err.response && err.response.data && err.response.data.message) {
      alert(err.response.data)
    } else {
      alert('An error occurred.')
    }
    console.error(err)
  }
}
const addLocation = async () => {
  try {
    const response = await axios.post('/api/locations', newLocation)
    var locationsRes = await axios.get(`/api/locations`)
    locations.value = locationsRes.data
    if (response.status === 200) {
      alert('Location created')
    } else {
      alert(response.data?.message || 'Error registering candidate.')
    }
  } catch (err) {
    if (err.response && err.response.data && err.response.data.message) {
      alert(err.response.data)
    } else {
      alert(err.response.data)
    }
    console.error(err)
  }
}
const addTemplate = async () => {
  try {
    newTemplate.questions = []
    const response = await axios.post('/api/interview_templates', newTemplate)
    var templateRes = await axios.get(`/api/interview_templates`)
    interviewTemplates.value = templateRes.data
    if (response.status === 200) {
      alert('Template created')
    } else {
      alert(response.data?.message || 'Error registering candidate.')
    }
  } catch (err) {
    if (err.response && err.response.data && err.response.data.message) {
      alert(err.response.data)
    } else {
      alert(err.response.data)
    }
    console.error(err)
  }
}
const addQuestiontoInterviewTemplate = async () => {
  try {
    if (!selectedTemplate.value.id || !questionIT.value) {
      alert('Please select a template and enter a question.')
      return
    }
    const response = await axios.post(
      `/api/interview_templates/${selectedTemplate.value.id}/questions`,
      `"${questionIT.value}"`,
      {
        headers: { 'Content-Type': 'application/json' },
      },
    )
    // Refresh selectedTemplate questions after adding
    const updatedTemplate = await axios.get(`/api/interview_templates/${selectedTemplate.value.id}`)
    selectedTemplate.value.questions = updatedTemplate.data.questions
    var templateRes = await axios.get(`/api/interview_templates`)
    interviewTemplates.value = templateRes.data
    if (response.status === 200) {
      alert('Question added')
    } else {
      alert(response.data?.message || 'Error adding question.')
    }
  } catch (err) {
    if (err.response && err.response.data && err.response.data.message) {
      alert(err.response.data.message)
    } else {
      alert('An error occurred.')
    }
    console.error(err)
  }
}
const addQuestionsToRecruitmentFormTemplate = async () => {
  try {
    if (!selectedRFTemplate.value.id || !questionRFT.value) {
      alert('Please select a template and enter a question.')
      return
    }
    const response = await axios.post(
      `/api/recruitment_form_templates/${selectedRFTemplate.value.id}/questions`,
      `"${questionRFT.value}"`,
      {
        headers: { 'Content-Type': 'application/json' },
      },
    )
    // Refresh selectedTemplate questions after adding
    const updatedTemplate = await axios.get(`/api/recruitment_form_templates/${selectedRFTemplate.value.id}`)
    selectedRFTemplate.value.questions = updatedTemplate.data.questions
    var templateRes = await axios.get(`/api/recruitment_form_templates`)
    recruitmentFormTemplates.value = templateRes.data
    if (response.status === 200) {
      alert('Question added')
    } else {
      alert(response.data?.message || 'Error adding question.')
    }
  } catch (err) {
    if (err.response && err.response.data && err.response.data.message) {
      alert(err.response.data.message)
    } else {
      alert('An error occurred.')
    }
    console.error(err)
  }
}
const updateCampaign = async () => {
  try {
    selectedCampaign.value.startDate = formatDate(selectedCampaign.value.startDate)
    selectedCampaign.value.endDate = formatDate(selectedCampaign.value.endDate)
    console.log(selectedCampaign.value)
    const response = await axios.patch(`/api/campaigns/${selectedCampaign.value.id}`, selectedCampaign.value)
    var campaignsRes = await axios.get(`/api/campaigns`)
    console.log(campaignsRes)
    campaigns.value = campaignsRes.data
    if (response.status === 200) {
      alert('Recruitment campaign updated')
    } else {
      alert(response.data?.message || 'Error registering candidate.')
    }
  } catch (err) {
    if (err.response && err.response.data && err.response.data.message) {
      alert(err.response.data)
    } else {
      alert(err.response.data)
    }
    console.error(err)
  }
}
</script>

<style scoped>
.recruitment-campaign-container {
  display: flex;
  flex-direction: column;
  height: 100%;
  max-width: 1200;
  margin: 0px auto;
  padding: 32px;
  background-color: #f9f9f9;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  font-family: 'Segoe UI', sans-serif;
}
.recruitment-campaign-container > div:first-child {
  flex-shrink: 0;
}

.display-campaigns {
  flex: 1;
  display: grid;
  grid-template-columns: repeat(1, 1fr);
  gap: 24px;
  min-height: 0;
}
.recruitment-campaign-buttons {
  flex: 1;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 24px;
  min-height: 0;
}
.table-container {
  display: flex;
  flex-direction: column;
  overflow-y: auto;
  border: 2px solid #ccc;
  border-radius: 8px;
  background: white;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.pretty-table {
  width: 100%;
  border-collapse: collapse;
  height: max-content;
  overflow-y: scroll;
}

.pretty-table th {
  background-color: #f4f4f4;
  position: sticky;
  top: 0;
  padding: 8px;
  border-bottom: 2px solid #ccc;
}

.pretty-table td {
  padding: 8px;
  border-bottom: 1px solid #eee;
}

.pretty-table tr:nth-child(even) {
  background-color: #fafafa;
}

.bold {
  font-weight: bold;
}

.primary-btn {
  background: #4caf50;
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
}

.primary-btn:hover {
  background: #45a049;
}

.change-status-btn,
.danger-btn {
  background: #e74c3c;
  color: white;
  font-size: small;
  border: none;
  padding: 4px 8px;
  border-radius: 0px;
  cursor: pointer;
}

.change-status-btn:hover,
.danger-btn {
  background: green;
}
#date {
  width: 100%;
  border-radius: 8px;
  padding: 10px;
  font-size: 14px;
  border: 1px solid #ccc;
}
</style>

<style>
.p-datepicker-panel {
  z-index: 2000 !important;
}
</style>