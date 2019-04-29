import axios from "axios";

const KFC_SSO_API = "https://api.kfc-sso.com";

export function generateApiKey(title, email) {
  return axios.post(`${KFC_SSO_API}/api/applications/generatekey`, {
    title: title,
    email: email,
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json"
    }
  });
}

export function updateApplicationInformation(
  key,
  title,
  description,
  logourl,
  underMaintenance
) {
  return axios.post(`${KFC_SSO_API}/api/applications/publish`, {
    key: key,
    title: title,
    description: description,
    logoUrl: logourl,
    underMaintenance: underMaintenance,
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json"
    }
  });
}
