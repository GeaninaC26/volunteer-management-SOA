<template>
  <div class="register-form-container" v-if="requeststatus === 1">
    <div class="register-form">
      <h2>
        Register Candidate for Campaign: {{ campaignName }}. </h2>
        <h2> This campaign started on
        {{ campaign.startDate }} and will end on {{ campaign.endDate }}
      </h2>

      <form @submit.prevent="submitForm">
        <div class="form-grid">
          <div class="form-item"><h3>First Name</h3><input v-model="candidate.firstName" placeholder="First Name" required /></div>
          <div class="form-item"><h3>Last Name</h3><input v-model="candidate.lastName" placeholder="Last Name" required /></div>
          <div class="form-item"><h3>Personal Email</h3><input v-model="candidate.personalEmail" placeholder="Email" type="email" required /></div>
          <div class="form-item"><h3>Phone Number</h3><input v-model="candidate.phone" placeholder="Phone" required /></div>
          <div class="form-item"><h3>Address</h3><input v-model="candidate.personalInfo.address" placeholder="Address" required /></div>
            <div class="form-item" >
            <h3>BirthDate</h3>
            <DatePicker
              v-model="candidate.personalInfo.birthDate"
              placeholder="Birth date"
              inputId="birthdate"
              dateFormat="yy-mm-dd"
              showIcon
            />
            </div>
          <div class="form-item"><h3>Facebook profile link</h3><input v-model="candidate.personalInfo.facebookProfile" placeholder="Facebook profile" required /></div>
          <div class="form-item"><h3>Instagram profile link</h3><input v-model="candidate.personalInfo.instagramProfile" placeholder="Instagram profile" required /></div>
          <div class="form-item"><h3>Allergies</h3><input v-model="candidate.personalInfo.allergies" placeholder="Allergies" required /></div>

          <div class="form-item"><h3>Gender</h3>
            <select v-model="candidate.personalInfo.gender" required>
              <option disabled value="">Select gender</option>
              <option v-for="gender in genders" :key="gender" :value="gender">{{ gender }}</option>
            </select>
          </div>
          <div class="form-item"><h3>Study Type</h3>
            <select v-model="candidate.personalInfo.studyType">
              <option disabled value="">Select study type</option>
              <option v-for="type in studyTypes" :key="type" :value="type">{{ type }}</option>
            </select>
          </div>
          <div class="form-item"><h3>Study Group</h3><input v-model="candidate.personalInfo.studyGroup" placeholder="StudyGroup" required /></div>

          <div class="form-item"><h3>Study Language</h3>
            <select v-model="candidate.personalInfo.studyLanguage">
              <option disabled value="">Select study language</option>
              <option v-for="lang in studyLanguages" :key="lang" :value="lang">{{ lang }}</option>
            </select>
          </div>
          <div class="form-item"><h3>Shirt size</h3>
            <select v-model="candidate.personalInfo.shirtSize">
              <option disabled value="">Select shirt size</option>
              <option v-for="size in shirtSizes" :key="size" :value="size">{{ size }}</option>
            </select>
          </div>
          <div class="form-item"><h3>Diet</h3>
            <select v-model="candidate.personalInfo.diet">
              <option disabled value="">Select diet</option>
              <option v-for="diet in diets" :key="diet" :value="diet">{{ diet }}</option>
            </select>
          </div>
          <div v-for="(question, idx) in registerFormQuestions" :key="idx">
          <div class="form-item">
            <h3>{{ question }}</h3>
            <input
              v-model="candidate.answersToForm[idx]"
              :placeholder="question"
              required
            />
          </div>
        </div>
        <div style="text-align:center; margin-top: 24px;">
          <button type="submit">Register</button>
        </div>
        </div>

      </form>
    </div>
  </div>
  <div class="register-form-container"  v-if="requeststatus === 4">
    <h1>This recruiting campaign has ended. Watch out for the next one.</h1>
  </div>
    <div class="register-form-container" v-if="requeststatus === 7">
    <h1>There is no recruiting campaign with this name. </h1>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'
import DatePicker from 'primevue/datepicker'

const route = useRoute()
const campaignName = route.params.campaignName
const registerFormQuestions = ref([])

const genders = ref([])
const studyTypes = ref([])
const studyLanguages = ref([])
const shirtSizes = ref([])
const diets = ref([])
const candidate = reactive({
  firstName: '',
  lastName: '',
  personalEmail: '',
  phone: '',
  personalInfo: {
    address: '',
    birthDate: '',
    facebookProfile: '',
    instagramProfile: '',
    allergies: '',
    gender: '',
    studyType: '',
    studyLanguage: '',
    studyGroup: '',
    shirtSize: '',
    diet: '',
  },
  recruitmentCampaignId: 0,
  answersToForm: []
})


const campaign = ref('')
const requeststatus = ref(0);
onMounted(async () => {
  const [
    genderRes,
    studyTypeRes,
    studyLangRes,
    shirtSizeRes,
    dietRes,
  ] = await Promise.all([
    axios.get('/api/type/gender'),
    axios.get('/api/type/study_type'),
    axios.get('/api/type/study_language'),
    axios.get('/api/type/shirt_size'),
    axios.get('/api/type/diet')
  ])

  var campaignRes = await axios.get(`/api/campaigns?name=${campaignName}`);
  if (campaignRes.data.length === 0){
    requeststatus.value = 7;
  } else {
    campaignRes = await axios.get(`/api/campaigns?name=${campaignName}&ongoing=${true}`);
    if(campaignRes.data.length === 0){
      requeststatus.value = 4;
    }else{
      requeststatus.value = 1;
    }
  }

  genders.value = genderRes.data
  studyTypes.value = studyTypeRes.data
  studyLanguages.value = studyLangRes.data
  shirtSizes.value = shirtSizeRes.data
  diets.value = dietRes.data
  candidate.recruitmentCampaignId = campaignRes.data[0].id

  campaign.value = campaignRes.data[0]
  console.log(campaign.value)

  var registerFormQuestionsRes = await axios.get(`/api/recruitment_form_templates/${campaign.value.recruitmentFormTemplateId}`)
  registerFormQuestions.value = registerFormQuestionsRes.data.questions
  console.log(registerFormQuestions)

})
const formatDate = (date) => {
  if (!date) return '';
  const d = new Date(date);
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

const submitForm = async () => {
  try {
    candidate.personalInfo.birthDate = formatDate(candidate.personalInfo.birthDate);
    const response = await axios.post(`/api/campaigns/${campaign.value.id}/candidates`, candidate);

    if (response.status === 200) {
      alert('Candidate registered!');
    } else {
      let err = ''
      for (let error of response.data?.errors || []) {
        for (let message of error) {
          err += message + '\n';
        }
      }
      alert(err || 'Error registering candidate.');
    }
  } catch (err) {
    console.error(err);
    if (err.response && err.response.data && err.response.data.errors) {
      let messages = [];
      for (let error of Object.entries(err.response.data.errors)) {
        for (let message of error) {
          messages.push(message);
        }
      }
      alert(messages.join('\n'));
    } else {
      alert('An unexpected error occurred.');
    }
  }
}
</script>

<style scoped>
.register-form-container {
  max-width: 1000px;
  margin: 0px auto;
  padding: 32px;
  background-color: #f9f9f9;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  font-family: "Segoe UI", sans-serif;
}

.register-form h2 {
  text-align: center;
  margin-bottom: 32px;
  color: #333;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 24px;
}

.form-item {
  display: flex;
  flex-direction: column;
}

.form-item h3 {
  margin-bottom: 8px;
  font-size: 16px;
  color: #333;
}

.form-item input,
.form-item select {
  padding: 10px 14px;
  border: 1px solid #ccc;
  border-radius: 8px;
  font-size: 14px;
  transition: border-color 0.2s ease;
}

.form-item input:focus,
.form-item select:focus {
  border-color: #7aa6ff;
  outline: none;
}

button[type="submit"] {
  background-color: #4e8cff;
  color: white;
  padding: 12px 28px;
  font-size: 16px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button[type="submit"]:hover {
  background-color: #3b72e0;
}

/* Date Picker (PrimeVue) */
#birthdate {
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