import axios from "axios";
import { api_url } from "@/const.js";

let config = {
  headers: {
    Token: localStorage.getItem("token")
  }
};

export function LogWebpageUsage(startTime, endTime, webpage) {
  const duration = endTime - startTime;
  const payload = {
    Page: webpage,
    DurationStart: startTime,
    DurationEnd: endTime,
    Duration: duration,
    Signature: "Signature",
    Salt: "salt"
  };
  config.headers.Token = localStorage.getItem("token");
  axios
    .post(`${api_url}/api/log/webpageusage`, payload, config)
    .then(response => {
      console.log(response.data);
    })
    .catch(err => {
      console.log(err.response.data);
    });
}
