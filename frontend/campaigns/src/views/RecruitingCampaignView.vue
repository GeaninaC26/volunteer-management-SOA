<!-- eslint-disable vue/require-v-for-key -->
<template>
  <div class="recruitment-campaign-container" v-if="requeststatus === 1">
    <div>
      <h2>Registered Candidates and volunteers for {{ campaignName }}.</h2>
      <h2>From {{ campaignStartDate }}, until {{ campaignEndDate }}</h2>
    </div>
    <div class="display-people">
      <!-- Volunteers table -->
      <div class="table-container">
        <table class="pretty-table">
          <thead>
            <tr class="table-head">
              <th colspan="3">Volunteers</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="volunteer in volunteers" :key="volunteer.id">
              <td>{{ volunteer.firstName }}</td>
              <td class="bold">{{ volunteer.department }}</td>
              <td>
                <button class="danger-btn" v-on:click="removeVolunteer(volunteer.id)">X</button>
              </td>
            </tr>
            <tr>
              <td colspan="100%"></td>
            </tr>
          </tbody>
        </table>
        <button
          class="primary-btn"
          data-bs-toggle="modal"
          data-bs-target="#addVolunteerModal"
          aria-controls="addVolunteerModal"
          @click="fetchVolunteersOutsideCampaign"
        >
          Add volunteer
        </button>
      </div>

      <!-- Candidates table -->
      <div class="table-container">
        <table class="pretty-table">
          <thead>
            <tr>
              <th colspan="100%">Candidates</th>
            </tr>
          </thead>
          <tbody>
          <tr v-for="candidate in candidates" :key="candidate.id">
            <td>{{ candidate.firstName }} {{ candidate.lastName }}</td>
            <td>
              <select v-model="candidate.recruitingStatus">
                <option v-for="status in recruitingStatuses" :key="status" :value="status">
                  {{ status }}
                </option>
              </select>
            </td>
            <td>
              <button
                class="change-status-btn"
                v-on:click="changeRecruitingStatus(candidate.id, candidate.recruitingStatus)"
              >
                Change status
              </button>
            </td>
          </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
  <div class="recruitment-campaign-container" v-if="requeststatus === 7">
    <div>
      <h2>There is no recruiting campaign with this name.</h2>
    </div>
  </div>
  <!-- Modal Add volunteers -->
  <div
    class="modal fade"
    id="addVolunteerModal"
    tabindex="-1"
    role="dialog"
    aria-labelledby="addVolunteerModal"
    aria-hidden="true"
  >
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="addVolunteerModalLabel">
            Search for the volunteer you want to add
          </h5>
        </div>
        <div class="modal-body">
          <input
            type="text"
            v-model="searchQuery"
            placeholder="Search volunteers..."
            @input="fetchVolunteersOutsideCampaign"
            style="width: 100%; padding: 6px; margin-bottom: 10px"
          />

          <div class="table-container">
            <table class="pretty-table">
              <thead>
                <tr></tr>
              </thead>
              <tr v-for="volunteer in volunteersOutsideCampaign" :key="volunteer.id">
                <td>
                  {{ volunteer.firstName }} {{ volunteer.lastName }} {{ volunteer.department }}
                </td>
                <td></td>
                <td>
                  <button
                    class="change-status-btn"
                    v-on:click="addVolunteerToCampaign(volunteer.id)"
                  >
                    Add volunteer
                  </button>
                </td>
              </tr>
            </table>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import * as signalR from "@microsoft/signalr";
import axios from 'axios'

const route = useRoute()
const campaignName = route.params.campaignName

const recruitingStatuses = ref([])
const volunteers = ref([])
const candidates = ref([])
const campaignStartDate = ref('')
const campaignEndDate = ref('')
const requeststatus = ref(0)
const campaignId = ref('')
const searchQuery = ref('')

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
    axios.get(`/api/campaigns/${campaignName}`).then((response) => {
      campaignId.value = response.data.id;
      campaignStartDate.value = response.data.startDate;
      campaignEndDate.value = response.data.endDate;
    });
  });
  connection.on("candidate_update", () => {
    console.log("candidate update received");
    axios.get(`/api/campaigns/${campaignId.value}/candidates`).then((response) => {
      candidates.value = response.data;
    });
  });
  connection.on("campaign_volunteer_update", () => {
    console.log("campaign volunteer update received");
    axios.get(`/api/campaigns/${campaignId.value}/volunteers`).then((response) => {
      volunteers.value = response.data;
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


  const recruitingStatusRes = await axios.get('/api/type/recruiting_status')
  var campaignRes = await axios.get(`/api/campaigns?name=${campaignName}`)
  if (campaignRes.data.length === 0) {
    requeststatus.value = 7
  } else {
    requeststatus.value = 1
  }
  campaignId.value = campaignRes.data[0].id
  console.log(campaignId.value)
  var volunteerCampaignRes = await axios.get(`/api/campaigns/${campaignId.value}/volunteers`)
  var candidatesCampaignRes = await axios.get(`/api/campaigns/${campaignId.value}/candidates`)
  volunteers.value = volunteerCampaignRes.data
  candidates.value = candidatesCampaignRes.data
  recruitingStatuses.value = recruitingStatusRes.data

  campaignStartDate.value = campaignRes.data[0].startDate

  campaignEndDate.value = campaignRes.data[0].endDate
})
// const reloadPage = () => {
//   window.location.reload()
// }
const removeVolunteer = async (idVolunteer) => {
  try {
    const response = await axios.delete(
      `/api/campaigns/${campaignId.value}/volunteers?id=${idVolunteer}`,
    )
    var volunteerCampaignRes = await axios.get(`/api/campaigns/${campaignId.value}/volunteers`)
    volunteers.value = volunteerCampaignRes.data
    if (response.status == 400) {
      alert('Volunteer succesfully removed.')
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
const volunteersOutsideCampaign = ref([])

watch(searchQuery, () => {
  fetchVolunteersOutsideCampaign()
})

const fetchVolunteersOutsideCampaign = async () => {
  try {
    const response = await axios.get(
      `/api/campaigns/${campaignId.value}/volunteers/?name=${searchQuery.value}&outside=${true}`,
    )
    console.log(response.data)
    volunteersOutsideCampaign.value = response.data
  } catch (error) {
    console.error('Error fetching volunteers outside campaign:', error)
  }
}
const changeRecruitingStatus = async (idCandidate, statusCandidate) => {
  try {
    console.log(statusCandidate)
    await axios.patch(`/api/candidates/${idCandidate}`, { recruitingStatus: statusCandidate })
    alert('Changed')
  } catch (error) {
    console.error("Couldn't change the status", error)
  }
}
const addVolunteerToCampaign = async (idVolunteer) => {
  try {
    console.log(idVolunteer)
    const response = await axios.post(
      `/api/campaigns/${campaignId.value}/volunteers?id=${idVolunteer}`,
      {
        headers: { 'Content-Type': 'application/json' },
      },
    )
    volunteersOutsideCampaign.value = response.data
    var volunteerCampaignRes = await axios.get(`/api/campaigns/${campaignId.value}/volunteers`)
    volunteers.value = volunteerCampaignRes.data
    fetchVolunteersOutsideCampaign()
    if (response.status === 200) {
      alert('Volunteer added')
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
  height: 82vh;
  max-width: 1000px;
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

.display-people {
  flex: 1;
  display: grid;
  grid-template-columns: repeat(2, 1fr);
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
</style>
