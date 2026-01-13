<template>
  <div class="schedule-container" v-if="requeststatus === 1">
    <div>
      <h2>Register Candidate for Campaign: {{ campaignName }}.</h2>
      <h2>From {{ campaignStartDate }}, until {{ campaignEndDate }}</h2>
    </div>

    <!-- Candidates table -->
    <div class="display-things">
      <div class="table-container">
        <table class="schedule-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Phone</th>
              <th>Interview date</th>
              <th>Interview time</th>
              <th>Volunteer that calls/sends sms</th>
            </tr>
          </thead>
          <tr v-for="candidate in candidates" :key="candidate.id">
            <td>{{ candidate.firstName }} {{ candidate.lastName }}</td>
            <td>{{ candidate.phone }}</td>
            <td></td>
            <td></td>
            <td>
              <select
                :value="candidate?.schedulerId || ''"
                @change="updateCandidateScheduler(candidate, $event.target.value)"
              >
                <option value="">-- Select Scheduler --</option>
                <option v-for="volunteer in volunteers" :key="volunteer.id" :value="volunteer.id">
                  {{ volunteer.firstName }} {{ volunteer.lastName }}
                </option>
              </select>
            </td>
          </tr>
        </table>
      </div>
    </div>

    <!-- Week navigation -->
    <div class="week-nav">
      <button @click="prevWeek" :disabled="currentWeekIndex === 0">Prev Week</button>
      <span>
        {{ weekRangeLabel }}
      </span>
      <button @click="nextWeek" :disabled="currentWeekIndex === weeks.length - 1">Next Week</button>
    </div>

    <!-- Schedule table (one week at a time) -->
    <div class="display-things">
      <div class="table-container">
        <table class="schedule-table">
          <thead>
            <tr>
              <th rowspan="2">Time</th>
              <th rowspan="2">Location</th>
              <th v-for="day in weeks[currentWeekIndex]" :key="day" colspan="3">
                {{ day.toLocaleDateString() }}
              </th>
            </tr>
          </thead>

          <tbody>
            <template v-for="time in timeSlots" :key="time">
              <tr v-for="(location, index) in locations" :key="time + location.id">
                <td v-if="index === 0" :rowspan="locations.length">
                  {{ time }}
                </td>
                <td>{{ location.name }}</td>

                <template v-for="day in weeks[currentWeekIndex]" :key="time + location.id + day">
                  <template v-if="isBlocked(day, time, location)">
                    <td class="blocked-cell" colspan="3">Blocked</td>
                  </template>
                  <template v-else>
                    <td>
                      <select
                        :value="
                          getDisponibilitiesForCell(day, time, location)[0]?.volunteerId || ''
                        "
                        @change="saveDisponibility(day, time, location, $event.target.value, 0)"
                      >
                        <option value="">-- Volunteer I --</option>
                        <option v-for="v in volunteers" :key="v.id" :value="v.id">
                          {{ v.firstName }} {{ v.lastName }}
                        </option>
                      </select>
                    </td>
                    <td>
                      <select
                        :value="
                          getDisponibilitiesForCell(day, time, location)[1]?.volunteerId || ''
                        "
                        @change="saveDisponibility(day, time, location, $event.target.value, 1)"
                      >
                        <option value="">-- Volunteer II --</option>
                        <option v-for="v in volunteers" :key="v.id" :value="v.id">
                          {{ v.firstName }} {{ v.lastName }}
                        </option>
                      </select>
                    </td>
                    <td>
                      <template v-if="getDisponibilitiesForCell(day, time, location).length === 2">
                        <select
                          :value="getInterviewForCell(day, time, location)?.candidateId || ''"
                          @change="createInterview(day, time, location, $event.target.value)"
                        >
                          <option value="">-- Candidate --</option>
                          <option v-for="c in candidates" :key="c.id" :value="c.id">
                            {{ c.firstName }} {{ c.lastName }}
                          </option>
                        </select>
                      </template>
                      <span v-else style="color: gray; font-style: italic">
                        Need 2 volunteers
                      </span>
                    </td>
                  </template>
                </template>
              </tr>
            </template>
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
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
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
const templateId = ref('')
const blockedPeriods = ref([])
const template = ref({ name: '', questions: '', duration: '' })
const locations = ref([])
const currentWeekIndex = ref(0)
const disponibilities = ref([])
const interviews = ref([])
onMounted(async () => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notifications")
    .withAutomaticReconnect()
    .build();
  
  connection.onclose(async () => {
    await connection.invoke("LeavePageGroup", route.path);
    await connection.invoke("LeavePageGroup", `/schedule_interviews`);
  });
  connection.on("campaign_update", () => {
    console.log("campaign update received");
    axios.get(`/api/campaigns/${campaignName}`).then((response) => {
      campaignId.value = response.data.id;
      campaignStartDate.value = response.data.startDate;
      campaignEndDate.value = response.data.endDate;
      templateId.value = response.data.interviewTemplateId;
    });
    axios.get(`/api/campaigns/${campaignId.value}/locations`).then((response) => {
      locations.value = response.data;
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
  connection.on("schedule_update", () => {
    console.log("schedule update received");
    axios.get(`/api/campaigns/${campaignId.value}/blocked_periods`).then((response) => {
      blockedPeriods.value = response.data;
    });
    axios.get(`/api/disponibilities`).then((response) => {
      disponibilities.value = response.data;
    });
    axios.get(`/api/interviews`).then((response) => {
      interviews.value = response.data;
    });
  });

  connection
    .start()
    .then(async () => {
      console.log("Connected to SignalR hub")
      await connection.invoke("JoinPageGroup", route.path);
      await connection.invoke("JoinPageGroup", `/schedule_interviews`);
    })
    .catch((err) => {
      console.error("SignalR connection error:", err);
    });

  const [recruitingStatusRes] = await Promise.all([
    axios.get('/api/type/recruiting_status'),
  ])

  const campaignRes = await axios.get(`/api/campaigns?name=${campaignName}`)
  if (campaignRes.data.length === 0) {
    requeststatus.value = 7
    return
  }

  const interviewsRes = await axios.get(`/api/interviews`)
  interviews.value = interviewsRes.data
  requeststatus.value = 1
  campaignId.value = campaignRes.data[0].id
  recruitingStatuses.value = recruitingStatusRes.data

  const [
    volunteerCampaignRes,
    candidatesCampaignRes,
    templateRes,
    blockedPeriodsRes,
    locationsRes,
  ] = await Promise.all([
    axios.get(`/api/campaigns/${campaignId.value}/volunteers`),
    axios.get(`/api/campaigns/${campaignId.value}/candidates`),
    axios.get(`/api/interview_templates`, templateId),
    axios.get(`/api/campaigns/${campaignId.value}/blocked_periods`),
    axios.get(`/api/campaigns/${campaignId.value}/locations`),
  ])
  const disponibilitiesRes = await axios.get('/api/disponibilities')
  disponibilities.value = disponibilitiesRes.data

  volunteers.value = volunteerCampaignRes.data
  candidates.value = candidatesCampaignRes.data
  campaignStartDate.value = campaignRes.data[0].startDate
  templateId.value = campaignRes.data[0].interviewTemplateId
  template.value = templateRes.data[0]
  blockedPeriods.value = blockedPeriodsRes.data
  locations.value = locationsRes.data
  campaignEndDate.value = campaignRes.data[0].endDate

  console.log(campaignRes)
})

const updateCandidateScheduler = async (candidate, volunteerId) => {
  if (!volunteerId) return

  const schedulerId = volunteers.value.find((v) => v.id === Number(volunteerId)).id
  if (!schedulerId) return

  try {
    await axios.patch(`/api/campaigns/${campaignId.value}/candidates/${candidate.id}`, {
      schedulerId, // full VolunteerDTO
    })

    candidate.schedulerId = schedulerId // keep UI in sync
  } catch (err) {
    console.error('Failed to update scheduler:', err)
    alert('Could not update candidate scheduler.')
  }
}

const getDisponibilitiesForCell = (day, time, location) => {
  const [hours, minutes] = time.split(':').map(Number)
  return disponibilities.value.filter((d) => {
    const dDate = new Date(d.dateTime)
    return (
      dDate.toDateString() === day.toDateString() &&
      dDate.getHours() === hours &&
      dDate.getMinutes() === minutes &&
      d.locationId === location.id
    )
  })
}

const saveDisponibility = async (day, time, location, volunteerId, index) => {
  if (!volunteerId) return

  const [hours, minutes] = time.split(':').map(Number)
  const slotDateObj = new Date(day)
  slotDateObj.setHours(hours, minutes, 0, 0)
  const slotDate = formatDateTime(slotDateObj)

  // find existing disponibility for this slot
  const existing = getDisponibilitiesForCell(day, time, location)[index]

  try {
    if (existing) {
      const dto = {
        volunteerId: Number(volunteerId),
        dateTime: slotDate,
        locationId: location.id,
      }
       await axios.patch(`/api/disponibilities/${existing.id}`, dto)
        console.log("ok")

    } else {
      // create new disponibility
      const dto = {
        volunteerId: Number(volunteerId),
        dateTime: slotDate,
        locationId: location.id,
      }
      await axios.post('/api/disponibilities', dto)

      // if (typeof res.data === 'object') {
      //   disponibilities.value.push(res.data)
      // } else {
      //   disponibilities.value.push({ id: res.data, ...dto })
      // }
    }
  const disponibilitiesRes = await axios.get('/api/disponibilities')
  disponibilities.value = disponibilitiesRes.data
  } catch (err) {
    console.error('Error saving disponibility:', err)
    alert('Failed to save disponibility.')
  }
}


const days = computed(() => generateDays(campaignStartDate.value, campaignEndDate.value))
const weeks = computed(() => splitIntoWeeks(days.value))
const timeSlots = computed(() => generateTimeSlots(Number(template.value.duration)))

const weekRangeLabel = computed(() => {
  if (!weeks.value.length) return ''
  const week = weeks.value[currentWeekIndex.value]
  const start = week[0]
  const end = week[week.length - 1]
  return `${start.toLocaleDateString()} - ${end.toLocaleDateString()}`
})

const blockedMap = computed(() => {
  const map = {}
  for (const bp of blockedPeriods.value) {
    const bpStart = new Date(bp.start)
    const [h, m] = bp.duration.split(':').map(Number)
    const duration = h * 60 + m
    const bpEnd = new Date(bpStart.getTime() + duration * 60 * 1000)

    const key = `${bpStart.toDateString()}_${bp.locationId}`
    if (!map[key]) map[key] = []
    map[key].push([bpStart, bpEnd])
  }
  return map
})

const isBlocked = (day, time, location) => {
  if (day < new Date(campaignStartDate.value) || day > new Date(campaignEndDate.value)) {
    return true
  }
  const [hours, minutes] = time.split(':').map(Number)
  const slotStart = new Date(day)
  slotStart.setHours(hours, minutes, 0, 0)

  const durationMinutes = Number(template.value.duration)
  const slotEnd = new Date(slotStart.getTime() + durationMinutes * 60 * 1000)

  const key = `${slotStart.toDateString()}_${location.id}`
  const blocks = blockedMap.value[key] || []

  return blocks.some(([bpStart, bpEnd]) => slotStart < bpEnd && slotEnd > bpStart)
}

const generateDays = (startDate, endDate) => {
  if (!startDate || !endDate) return []
  const days = []
  let current = new Date(startDate)
  while (current <= new Date(endDate)) {
    days.push(new Date(current))
    current.setDate(current.getDate() + 1)
  }
  return days
}

const splitIntoWeeks = (allDays) => {
  if (!allDays.length) return []
  const weeks = []
  let currentWeek = []
  for (let day of allDays) {
    if (currentWeek.length === 0) {
      // Align to Monday start
      const dayOfWeek = day.getDay() === 0 ? 7 : day.getDay()
      for (let i = 1; i < dayOfWeek; i++) {
        currentWeek.push(new Date(day.getTime() - (dayOfWeek - i) * 24 * 60 * 60 * 1000))
      }
    }
    currentWeek.push(day)
    if (currentWeek.length === 7) {
      weeks.push(currentWeek)
      currentWeek = []
    }
  }
  if (currentWeek.length) weeks.push(currentWeek)
  return weeks
}

const generateTimeSlots = (durationMinutes) => {
  if (!durationMinutes) return []
  const slots = []
  const start = 10 * 60 // 10:00
  const end = 22 * 60 // 22:00

  for (let t = start; t + durationMinutes <= end; t += durationMinutes) {
    const hours = String(Math.floor(t / 60)).padStart(2, '0')
    const mins = String(t % 60).padStart(2, '0')
    slots.push(`${hours}:${mins}`)
  }
  return slots
}

const prevWeek = () => {
  if (currentWeekIndex.value > 0) currentWeekIndex.value--
}

const nextWeek = () => {
  if (currentWeekIndex.value < weeks.value.length - 1) currentWeekIndex.value++
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

const getInterviewForCell = (day, time, location) => {
  const [hours, minutes] = time.split(':').map(Number)
  const slotDate = new Date(day)
  slotDate.setHours(hours, minutes, 0, 0)

  return interviews.value.find(
    (i) =>
      new Date(i.dateTime).toDateString() === slotDate.toDateString() &&
      new Date(i.dateTime).getHours() === slotDate.getHours() &&
      new Date(i.dateTime).getMinutes() === slotDate.getMinutes() &&
      i.locationId === location.id
  )
}


const createInterview = async (day, time, location, candidateId) => {
  if (!candidateId) return

  const candidate = candidates.value.find((c) => c.id === Number(candidateId))
  console.log(candidate);

  if (!candidate?.schedulerId) {
    alert('This candidate does not have a scheduler assigned.')
    return
  }

  const volunteersForSlot = getDisponibilitiesForCell(day, time, location)
  if (volunteersForSlot.length !== 2) {
    alert('You must select exactly 2 volunteers before scheduling an interview.')
    return
  }


  const [hours, minutes] = time.split(':').map(Number)
  var slotDate = new Date(day)
  slotDate.setHours(hours, minutes, 0, 0)
  slotDate = formatDateTime(slotDate)

  // ✅ If candidate already has an interview, delete it
  const existingInterview = interviews.value.find((i) => i.candidateId === Number(candidateId))
  console.log(existingInterview);
  if (existingInterview) {
    await axios.delete(`/api/interviews/${existingInterview.id}`)
    interviews.value = interviews.value.filter((i) => i.id !== existingInterview.id)
  }

  const interviewerDTOs = volunteersForSlot
    .map(d => volunteers.value.find(v => v.id === d.volunteerId))
    .filter(Boolean)

  const dto = {
    candidateId: Number(candidateId),
    interviewers: interviewerDTOs,
    locationId: location.id,
    dateTime: slotDate,
    notes: ''
  }

  try {
    const res = await axios.post('/api/interviews', dto)
    interviews.value.push({ id: res.data.id, ...dto }) // keep local state in sync
    alert('Interview successfully created!')
  } catch (err) {
    console.error('Error creating interview:', err)
    alert('Failed to create interview.')
  }
}


</script>

<style scoped>
.schedule-container {
  display: flex;
  flex-direction: column;
  max-width: 1500px;
  margin: 0 auto;
  padding: 32px;
  background-color: #f9f9f9;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  font-family: 'Segoe UI', sans-serif;
}

.display-things {
  flex: 1;
  display: grid;
  gap: 24px;
  min-height: 0;
}

.table-container {
  display: flex;
  flex-direction: column;
  overflow-y: auto;
  border: 2px solid #ccc;
  max-height: 500px;
  margin-bottom: 5%;
  border-radius: 8px;
  background: white;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.schedule-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

.schedule-table th {
  background-color: #f4f4f4;
  position: sticky;
  top: 0;
  padding: 8px;
  z-index: 5;
  border: 1px solid #ccc;
}

.schedule-table td {
  border: 1px solid #ccc;
  padding: 12px 16px;
}

.week-nav {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 16px;
  margin: 16px 0;
}

.blocked-cell {
  background: #f0f0f0;
  text-align: center;
  color: #777;
  font-style: italic;
}
</style>
