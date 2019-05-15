import axios from "axios";
import { api_url } from "@/const.js";

export function GetAnalyticsData() {
  return axios.get(`${api_url}/api/analytics/usage`);
}

const months = 6;

export function GetSuccessfulLoginsxRegisteredUsers(data) {
  const successData = data.map(month => {
    var monthData = {};
    const date = new Date(month.month + "-" + "1" + "-" + month.year);
    monthData = {
      date: date,
      totalRegisteredUsers: month.totalRegisteredUsers,
      loginAttempts: month.loginAttempts
    };
    return monthData;
  });
  // Order Desc by date
  successData.sort((a, b) => {
    return b.date - a.date;
  });
  return successData.slice(0, months);
}

export function FailedvsSucessfulLoginAttempts(data) {
  const loginData = Object.keys(data).map(key => {
    const month = data[key];
    var monthData = {};
    const date = new Date(month.month + "-" + "1" + "-" + month.year);
    const total = month.successfulLoginAttempts + month.failedLoginAttempts;
    monthData = {
      date: date,
      failedAttempts: month.failedLoginAttempts,
      loginAttempts: month.successfulLoginAttempts,
      totalAttempts: total
    };
    return monthData;
  });
  // Order Desc by date
  loginData.sort((a, b) => {
    return b.date - a.date;
  });
  return loginData.slice(0, months);
}

export function GetTopFeaturesByPageVisits(data) {
  const rank = [5, 4, 3, 2, 1];
  const rankData = data.map((feature, i) => {
    feature["rank"] = rank[i];
    return feature;
  });
  return rankData.slice(0, 5);
}

export function GetTopPagesByPageTime(data) {
  const rank = [5, 4, 3, 2, 1];
  const rankData = data.map((page, i) => {
    page["rank"] = rank[i];
    return page;
  });
  return rankData.slice(0, 5);
}

export function GetAverageSessionDuration6Months(data) {
  // Sort From Recent
  const usageData = data.map(month => {
    var monthData = {};
    const date = new Date(month.month + "-" + "1" + "-" + month.year);
    monthData = {
      date: date,
      duration: month.sessionDuration
    };
    return monthData;
  });
  const orderedDesc = usageData.sort((a, b) => {
    return b.date - a.date;
  });
  return orderedDesc.slice(0, months);
}
