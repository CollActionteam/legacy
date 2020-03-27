import { RouteComponentProps } from "react-router-dom";
import React from "react";

type TParams = {
  slug: string,
  projectId: string
}

const ThankYouCommitPage = ({ match } : RouteComponentProps<TParams>): any => {
  return <div>Thank you for participating in this project</div>; // TODO
}

export default ThankYouCommitPage;