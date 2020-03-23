import { RouteComponentProps } from "react-router-dom";
import React from "react";

type TParams = {
  slug: string,
  projectId: string
}

const ThankYouCommitPage = ({ match } : RouteComponentProps<TParams>): any => {
  return <div></div>;
}

export default ThankYouCommitPage;