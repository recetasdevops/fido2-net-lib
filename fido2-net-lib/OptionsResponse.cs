﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;
using static fido2NetLib.Fido2NetLib;

namespace fido2NetLib
{
    public class CredentialCreateOptions
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 
        /// This member contains data about the Relying Party responsible for the request.
        /// Its value’s name member is required.
        /// Its value’s id member specifies the relying party identifier with which the credential should be associated.If omitted, its value will be the CredentialsContainer object’s relevant settings object's origin's effective domain.
        /// </summary>
        [JsonProperty("rp")]
        public Rp Rp { get; set; }

        /// <summary>
        /// This member contains data about the user account for which the Relying Party is requesting attestation. 
        /// Its value’s name, displayName and id members are required.
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }

        /// <summary>
        /// Must be generated by the Server (Relying Party)
        /// </summary>
        [JsonProperty("challenge")]
        [JsonConverter(typeof(Base64UrlConverter))]
        public byte[] Challenge { get; set; }

        /// <summary>
        /// This member contains information about the desired properties of the credential to be created. The sequence is ordered from most preferred to least preferred. The platform makes a best-effort to create the most preferred credential that it can.
        /// </summary>
        [JsonProperty("pubKeyCredParams")]
        public List<PubKeyCredParam> PubKeyCredParams { get; set; }

        /// <summary>
        /// This member specifies a time, in milliseconds, that the caller is willing to wait for the call to complete. This is treated as a hint, and MAY be overridden by the platform.
        /// </summary>
        [JsonProperty("timeout")]
        public long Timeout { get; set; }

        // todo: add more members from https://w3c.github.io/webauthn/#dom-publickeycredentialcreationoptions-pubkeycredparams

        private static PubKeyCredParam ES256 = new PubKeyCredParam()
        {
            // External authenticators support the ES256 algorithm
            Type = "public-key",
            Alg = -7
        };

        private static PubKeyCredParam RS256 = new PubKeyCredParam()
        {
            // Windows Hello supports the RS256 algorithm
            Type = "public-key",
            Alg = -257
        };

        /// <summary>
        /// This member is intended for use by Relying Parties that wish to express their preference for attestation conveyance.The default is none.
        /// </summary>
        [JsonProperty("attestation")]
        public string Attestation { get; set; } = "none";

        public AuthenticatorSelection AuthenticatorSelection { get; set; }

        /// <summary>
        /// This member is intended for use by Relying Parties that wish to limit the creation of multiple credentials for the same account on a single authenticator.The client is requested to return an error if the new credential would be created on an authenticator that also contains one of the credentials enumerated in this parameter.
        /// </summary>
        [JsonProperty("excludeCredentials")]
        public List<PublicKeyCredentialDescriptor> ExcludeCredentials { get; set; } = new List<PublicKeyCredentialDescriptor>();

        public static CredentialCreateOptions Create(byte[] challenge, Configuration config, AuthenticatorSelection authenticatorSelection)
        {
            return new CredentialCreateOptions
            {
                Status = "ok",
                ErrorMessage = string.Empty,
                Challenge = challenge,
                Rp = new Rp(config.ServerDomain, config.ServerName),
                Timeout = config.Timeout,
                PubKeyCredParams = new List<PubKeyCredParam>()
                {
                    ES256, // todo: support more formats tha es256
                    RS256
                },
                AuthenticatorSelection = authenticatorSelection
                
            };
        }
    }

    public class PubKeyCredParam
    {
        /// <summary>
        /// The type member specifies the type of credential to be created.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The alg member specifies the cryptographic signature algorithm with which the newly generated credential will be used, and thus also the type of asymmetric key pair to be generated, e.g., RSA or Elliptic Curve.
        /// </summary>
        [JsonProperty("alg")]
        public long Alg { get; set; }
    }

    public class Rp
    {
        public Rp(string id, string name)
        {
            Name = name;
            Id = id;
        }

        /// <summary>
        /// A human-readable name for the entity. Its function depends on what the PublicKeyCredentialEntity represents:
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A unique identifier for the Relying Party entity, which sets the RP ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class AuthenticatorSelection
    {
        public string AuthenticatorAttachment { get; set; }
        public bool RequireResidentKey { get; set; }
        public string UserVerification { get; set; }
    }

    public class User
    {

        /// <summary>
        /// Required. A human-friendly identifier for a user account. It is intended only for display, i.e., aiding the user in determining the difference between user accounts with similar displayNames. For example, "alexm", "alex.p.mueller@example.com" or "+14255551234". https://w3c.github.io/webauthn/#dictdef-publickeycredentialentity
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The user handle of the user account entity. To ensure secure operation, authentication and authorization decisions MUST be made on the basis of this id member, not the displayName nor name members
        /// </summary>
        [JsonProperty("id")]
        [JsonConverter(typeof(Base64UrlConverter))]
        public byte[] Id { get; set; }

        /// <summary>
        /// A human-friendly name for the user account, intended only for display. For example, "Alex P. Müller" or "田中 倫". The Relying Party SHOULD let the user choose this, and SHOULD NOT restrict the choice more than necessary.
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}
