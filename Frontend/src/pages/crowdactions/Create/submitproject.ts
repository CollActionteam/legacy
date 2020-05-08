import { ExecutionResult, gql, useMutation } from "@apollo/client";
import { ICrowdactionResult } from "../../../api/types";
import { ICrowdactionForm } from "./form";
import Utils from "../../../utils";

async function useSubmitCrowdaction(form: ICrowdactionForm): Promise<ICrowdactionResult> {
  let bannerId;
  if (form.banner) {
    bannerId = await Utils.uploadImage(form.banner, form.crowdactionName, 1600);
  }

  let imageId;
  if (form.image) {
    imageId = await Utils.uploadImage(form.image, form.imageDescription, 1600);
  }

  let cardId;
  if (form.banner) {
    cardId = await Utils.uploadImage(form.banner, form.crowdactionName, 370);
  }

  const [createCrowdaction] = useMutation(gql`
    mutation Create($crowdaction: NewCrowdactionInputGraph) {
      crowdaction {
        createCrowdaction(crowdaction: $crowdaction) {
          succeeded
          errors {
            errorMessage
            memberNames
          }
        }
      }
    }
  `);

  const response = (await createCrowdaction({
    variables: {
      crowdaction: {
        name: form.crowdactionName,
        bannerImageFileId: bannerId || null,
        categories: [form.category],
        target: form.target,
        proposal: form.proposal,
        description: form.description,
        start: form.startDate,
        end: form.endDate,
        goal: form.goal,
        tags: form.tags ? form.tags.split(';') : [],
        creatorComments: form.comments || null,
        descriptiveImageFileId: imageId || null,
        cardImageFileId: cardId || null,
        descriptionVideoLink: form.youtube || null
      }
    }
  })) as ExecutionResult;

  if (!response || !response.data) {
    throw new Error('No response from graphql endpoint');
  }

  return response.data.crowdaction.createCrowdaction;
}

export default useSubmitCrowdaction;