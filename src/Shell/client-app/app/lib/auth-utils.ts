const visitedMicroservices: string[] = [];

export function addVisitedMicroservice(baseURL: string) {
  if (!visitedMicroservices.includes(baseURL)) {
    visitedMicroservices.push(baseURL);
  }
}

export function getVisitedMicroservices(): string[] {
  return [...visitedMicroservices];
}